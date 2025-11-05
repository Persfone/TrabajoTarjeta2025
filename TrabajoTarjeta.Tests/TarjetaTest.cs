// TrabajoTarjeta.Tests/TarjetaTest.cs
using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using TrabajoTarjeta;

namespace TrabajoTarjeta.Tests
{
    [TestFixture]
    public class TarjetaTest
    {
        [SetUp]
        public void Setup()
        {
            Tiempo.Now = () => new DateTime(2025, 4, 5, 10, 0, 0); // Lunes 10:00
        }

        [Test]
        public void Tarjeta_Constructor_ValoresPorDefecto()
        {
            var tarjeta = new Tarjeta();
            Assert.AreEqual(0, tarjeta.Saldo);
            Assert.AreEqual("Sin Franquicia", tarjeta.TipoTarjeta);
            Assert.IsTrue(tarjeta.Id.StartsWith("SUBE-"));
            Assert.AreEqual(0, tarjeta.SaldoPendiente);
        }

        [Test]
        public void CargarTarjeta_CargaExitosa_SaldoActualizado()
        {
            var tarjeta = new Tarjeta();
            var input = new StringReader("1\n");
            tarjeta = tarjeta.CargarTarjeta(tarjeta, input);
            Assert.AreEqual(2000, tarjeta.Saldo);
        }

        [Test]
        public void CargarTarjeta_CargaConExcedente_SaldoPendienteActualizado()
        {
            var tarjeta = new Tarjeta();
            tarjeta.Saldo = Tarjeta.SALDO_MAXIMO - 1000;
            var input = new StringReader("1\n");
            tarjeta = tarjeta.CargarTarjeta(tarjeta, input);
            Assert.AreEqual(Tarjeta.SALDO_MAXIMO, tarjeta.Saldo);
            Assert.AreEqual(1000, tarjeta.SaldoPendiente);
        }

        [Test]
        public void CargarTarjeta_OpcionInvalida_RepiteHastaValida()
        {
            var tarjeta = new Tarjeta();
            var input = new StringReader("10\n99\n1\n");
            tarjeta = tarjeta.CargarTarjeta(tarjeta, input);
            Assert.AreEqual(2000, tarjeta.Saldo);
        }

        [Test]
        public void AcreditarCarga_SaldoPendienteExistente_SaldoActualizado()
        {
            var tarjeta = new Tarjeta();
            tarjeta.Saldo = Tarjeta.SALDO_MAXIMO - 500;
            tarjeta.SaldoPendiente = 1000;
            tarjeta.AcreditarCarga();
            Assert.AreEqual(Tarjeta.SALDO_MAXIMO, tarjeta.Saldo);
            Assert.AreEqual(500, tarjeta.SaldoPendiente);
        }

        [Test]
        public void AcreditarCarga_SinSaldoPendiente_NoCambios()
        {
            var tarjeta = new Tarjeta();
            tarjeta.Saldo = 1000;
            double saldoInicial = tarjeta.Saldo;
            tarjeta.AcreditarCarga();
            Assert.AreEqual(saldoInicial, tarjeta.Saldo);
            Assert.AreEqual(0, tarjeta.SaldoPendiente);
        }

        [Test]
        public void Pagar_SaldoSuficiente_PagoExitoso()
        {
            var tarjeta = new Tarjeta();
            tarjeta.Saldo = 2000;
            var colectivo = new Colectivo("Linea 1", false);
            bool resultado = tarjeta.Pagar(500, colectivo);
            Assert.IsTrue(resultado);
            Assert.AreEqual(1500, tarjeta.Saldo);
        }

        [Test]
        public void Pagar_ConSaldoNegativoDentroLimite_PagoExitoso()
        {
            var tarjeta = new Tarjeta();
            tarjeta.Saldo = -1000;
            var colectivo = new Colectivo("Linea 1", false);
            bool resultado = tarjeta.Pagar(100, colectivo);
            Assert.IsTrue(resultado);
            Assert.AreEqual(-1100, tarjeta.Saldo);
        }

        [Test]
        public void Pagar_TrasbordoDentroDe1Hora_MismoDiaLaboral_NoSeCobra()
        {
            var tarjeta = new Tarjeta();
            tarjeta.Saldo = 2000;
            tarjeta.fechaUltimoViaje = Tiempo.Now().AddMinutes(-50);
            tarjeta.ultimaLinea = "Linea 1";
            var colectivo = new Colectivo("Linea 2", false);

            bool resultado = tarjeta.Pagar(Colectivo.TARIFA_BASICA, colectivo);

            Assert.IsTrue(resultado);
            Assert.AreEqual(2000, tarjeta.Saldo);
        }

        [Test]
        public void Pagar_TrasbordoFueraDe1Hora_SeCobraCompleto()
        {
            var tarjeta = new Tarjeta();
            tarjeta.Saldo = 2000;
            tarjeta.fechaUltimoViaje = Tiempo.Now().AddHours(-2);
            tarjeta.ultimaLinea = "Linea 1";
            var colectivo = new Colectivo("Linea 2", false);

            bool resultado = tarjeta.Pagar(Colectivo.TARIFA_BASICA, colectivo);

            Assert.IsTrue(resultado);
            Assert.AreEqual(2000 - Colectivo.TARIFA_BASICA, tarjeta.Saldo);
        }

        [Test]
        public void SinFranquicia_UsoFrecuente_DesdeViaje30_Descuento80()
        {
            var tarjeta = new SinFranquicia();
            typeof(SinFranquicia)
                .GetField("cantidadViajes", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(tarjeta, 30);

            float descuento = tarjeta.UsoFrecuente();
            Assert.AreEqual(0.8f, descuento);
        }

        [Test]
        public void MedioBoletoEstudiantil_Pagar_DosViajesConDescuento()
        {
            DateTime tiempoActual = new DateTime(2025, 4, 5, 10, 0, 0);
            Tiempo.Now = () => tiempoActual;

            var tarjeta = new MedioBoletoEstudiantil();
            tarjeta.Saldo = 5000;
            var colectivo = new Colectivo("Linea 1", false);

            // Primer viaje
            bool r1 = tarjeta.Pagar(Colectivo.TARIFA_BASICA, colectivo);

            // Avanzar 6 minutos
            tiempoActual = tiempoActual.AddMinutes(6);
            Tiempo.Now = () => tiempoActual;

            // Segundo viaje
            bool r2 = tarjeta.Pagar(Colectivo.TARIFA_BASICA, colectivo);

            Assert.IsTrue(r1);
            Assert.IsTrue(r2);
            Assert.AreEqual(5000 - (Colectivo.TARIFA_BASICA * 0.5 * 2), tarjeta.Saldo, 0.01);
        }

        [Test]
        public void MedioBoletoEstudiantil_EsperaMenosDe5Min_PagoFallido()
        {
            // --- CONTROL DE TIEMPO FIJO ---
            DateTime tiempoActual = new DateTime(2025, 4, 5, 10, 0, 0);
            Tiempo.Now = () => tiempoActual;

            var tarjeta = new MedioBoletoEstudiantil();
            tarjeta.Saldo = 5000;
            var colectivo = new Colectivo("Linea 1", false);

            // Primer viaje
            tarjeta.Pagar(Colectivo.TARIFA_BASICA, colectivo);

            // Avanzar solo 3 minutos
            tiempoActual = tiempoActual.AddMinutes(3);
            Tiempo.Now = () => tiempoActual;

            // Segundo intento → debe fallar por <5 min
            bool resultado = tarjeta.Pagar(Colectivo.TARIFA_BASICA, colectivo);

            Assert.IsFalse(resultado);
        }

        [Test]
        public void FranquiciaCompleta_FueraDeHorario_SeCobra()
        {
            Tiempo.Now = () => new DateTime(2025, 4, 5, 23, 0, 0);
            var tarjeta = new FranquiciaCompleta();
            tarjeta.Saldo = 2000;
            var colectivo = new Colectivo("Linea 1", false);

            bool resultado = tarjeta.Pagar(Colectivo.TARIFA_BASICA, colectivo);

            Assert.IsTrue(resultado);
            Assert.AreEqual(2000 - Colectivo.TARIFA_BASICA, tarjeta.Saldo);
        }
    }
}