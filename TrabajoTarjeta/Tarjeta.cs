// TrabajoTarjeta/Tarjeta.cs
using System;
using System.IO;

namespace TrabajoTarjeta
{
    public class Tarjeta
    {
        public double Saldo { get; set; }
        public string TipoTarjeta { get; set; }
        public string Id { get; set; }
        public DateTime fechaUltimoViaje = DateTime.MinValue;
        public string ultimaLinea;
        public const double SALDO_NEGATIVO = 1200;
        public const double SALDO_MAXIMO = 56000;
        public double SaldoPendiente { get; set; }
        private readonly int[] saldos = { 2000, 3000, 4000, 5000, 10000, 15000, 20000, 25000, 30000 };

        public Tarjeta()
        {
            Saldo = 0;
            TipoTarjeta = "Sin Franquicia";
            Id = $"SUBE-{Guid.NewGuid()}";
            SaldoPendiente = 0;
        }

        public void CargarTarjeta(TextReader input = null)
        {
            input ??= Console.In;
            bool cargoTarjeta = false;
            while (!cargoTarjeta)
            {
                Console.WriteLine("¿Cuánto desea cargar?:\n");
                for (int i = 0; i < saldos.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {saldos[i]}");
                }
                Console.Write("\n\nSeleccione una opción: ");
                string opc = input.ReadLine();
                if (int.TryParse(opc, out int opcion))
                {
                    opcion -= 1;
                    if (opcion >= 0 && opcion < saldos.Length)
                    {
                        double montoACcargar = saldos[opcion];
                        if (Saldo + montoACcargar <= SALDO_MAXIMO)
                        {
                            cargoTarjeta = true;
                            Console.WriteLine($"Cargaste {saldos[opcion]}");
                            Saldo += saldos[opcion];
                        }
                        else
                        {
                            double espacioDisponible = SALDO_MAXIMO - Saldo;
                            double excedente = montoACcargar - espacioDisponible;
                            Saldo = SALDO_MAXIMO;
                            SaldoPendiente += excedente;
                            cargoTarjeta = true;
                            Console.WriteLine($"Se acreditaron ${espacioDisponible} y quedaron ${excedente} pendientes de acreditación.");
                            Console.WriteLine($"Saldo actual: ${Saldo}");
                            Console.WriteLine($"Saldo pendiente: ${SaldoPendiente}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Opción inválida");
                    }
                }
                else
                {
                    Console.WriteLine("Ingrese un número válido");
                }
            }
        }

        public void AcreditarCarga()
        {
            if (SaldoPendiente > 0)
            {
                double espacioDisponible = SALDO_MAXIMO - Saldo;
                if (espacioDisponible > 0)
                {
                    double montoAAcreditar = Math.Min(SaldoPendiente, espacioDisponible);
                    Saldo += montoAAcreditar;
                    SaldoPendiente -= montoAAcreditar;
                    Console.WriteLine($"Se acreditaron ${montoAAcreditar} del saldo pendiente.");
                    Console.WriteLine($"Saldo actual: ${Saldo}");
                    Console.WriteLine($"Saldo pendiente restante: ${SaldoPendiente}");
                }
            }
        }

        public virtual bool Pagar(double monto, Colectivo colectivo)
        {
            bool esTrasbordo = false;
            if (Saldo + SALDO_NEGATIVO >= monto)
            {
                var ahora = Tiempo.Now();
                if ((ahora - fechaUltimoViaje).TotalHours < 1
                    && ahora.DayOfWeek != DayOfWeek.Sunday
                    && ahora.Hour >= 7 && ahora.Hour < 22
                    && ultimaLinea != null
                    && ultimaLinea != colectivo.ObtenerLinea())
                {
                    esTrasbordo = true;
                }

                if (esTrasbordo)
                {
                    Console.WriteLine("Trasbordo gratuito realizado.");
                }
                else
                {
                    Saldo -= monto;
                }

                fechaUltimoViaje = ahora;
                ultimaLinea = colectivo.ObtenerLinea();
                AcreditarCarga();
                return true;
            }
            return false;
        }
    }

    public class SinFranquicia : Tarjeta
    {
        private int cantidadViajes = 0;

        public float UsoFrecuente()
        {
            float descuento = 1f;
            if (Tiempo.Now().Day == 1)
            {
                cantidadViajes = 0;
            }
            if (cantidadViajes <= 29)
                descuento = 1f;
            else if (cantidadViajes <= 59)
                descuento = 0.8f;
            else if (cantidadViajes <= 80)
                descuento = 0.75f;
            else
                descuento = 1f;
            return descuento;
        }

        public override bool Pagar(double monto, Colectivo colectivo)
        {
            bool pagoExitoso = base.Pagar(monto * UsoFrecuente(), colectivo);
            if (pagoExitoso)
            {
                cantidadViajes++;
            }
            return pagoExitoso;
        }
    }

    public class MedioBoletoEstudiantil : Tarjeta
    {
        private int viajesHoy = 0;

        public MedioBoletoEstudiantil()
        {
            TipoTarjeta = "Medio Boleto Estudiantil";
        }

        public override bool Pagar(double monto, Colectivo colectivo)
        {
            var ahora = Tiempo.Now();
            if (ahora.Hour < 6 || ahora.Hour > 22)
            {
                Console.WriteLine("El medio boleto estudiantil solo es válido entre las 6:00 y las 22:00 horas.");
                return base.Pagar(monto, colectivo);
            }

            DateTime hoy = ahora.Date;
            if (fechaUltimoViaje.Date != hoy)
            {
                viajesHoy = 0;
            }

            if ((ahora - fechaUltimoViaje).TotalMinutes < 5)
            {
                Console.WriteLine("Debe esperar al menos 5 minutos antes de usar nuevamente la tarjeta.");
                return false;
            }

            double montoAPagar;
            if (viajesHoy < 2)
            {
                montoAPagar = monto * 0.5;
                viajesHoy++;
                Console.WriteLine($"Viaje con descuento #{viajesHoy} del día.");
            }
            else
            {
                montoAPagar = monto;
                Console.WriteLine("Ya utilizaste los 2 medios boletos del día. Este viaje se cobra completo.");
            }

            bool pagoExitoso = base.Pagar(montoAPagar, colectivo);
            if (!pagoExitoso && viajesHoy > 0)
            {
                viajesHoy--;
            }
            return pagoExitoso;
        }
    }

    public class BoletoGratuitoEstudiantil : Tarjeta
    {
        private int viajesHoy = 0;

        public BoletoGratuitoEstudiantil()
        {
            TipoTarjeta = "Boleto Gratuito Estudiantil";
        }

        public override bool Pagar(double monto, Colectivo colectivo)
        {
            var ahora = Tiempo.Now();
            if (ahora.Hour < 6 || ahora.Hour > 22)
            {
                Console.WriteLine("El boleto gratuito estudiantil solo es válido entre las 6:00 y las 22:00 horas.");
                return base.Pagar(monto, colectivo);
            }

            DateTime hoy = ahora.Date;
            if (fechaUltimoViaje.Date != hoy)
            {
                viajesHoy = 0;
            }

            if (viajesHoy < 2)
            {
                viajesHoy++;
                Console.WriteLine($"Viaje gratuito #{viajesHoy} del día ({hoy:dd/MM/yyyy})");
                return base.Pagar(0, colectivo);
            }
            else
            {
                Console.WriteLine("Ya utilizaste los 2 boletos gratuitos del día. Este viaje se cobra completo.");
                return base.Pagar(monto, colectivo);
            }
        }
    }

    public class FranquiciaCompleta : Tarjeta
    {
        public FranquiciaCompleta()
        {
            TipoTarjeta = "Franquicia Completa";
        }

        public override bool Pagar(double monto, Colectivo colectivo)
        {
            var ahora = Tiempo.Now();
            if (ahora.Hour < 6 || ahora.Hour > 22)
            {
                Console.WriteLine("La franquicia completa solo es válida entre las 6:00 y las 22:00 horas.");
                return base.Pagar(monto, colectivo);
            }

            Console.WriteLine("Viaje gratuito por franquicia completa.");
            return base.Pagar(0, colectivo);
        }
    }
}