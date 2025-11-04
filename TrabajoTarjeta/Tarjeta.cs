using System;
using System.Threading;

namespace TrabajoTarjeta
{
    public class Tarjeta
    {
        public double Saldo { get; set; }
        public string TipoTarjeta { get; set; }
        public string Id { get; set; }
        public DateTime fechaUltimoViaje = DateTime.MinValue;
        public const double SALDO_NEGATIVO = 1200;


        private readonly int[] saldos = { 2000, 3000, 4000, 5000, 10000, 15000, 20000, 25000, 30000 };

        public Tarjeta()
        {
            Saldo = 0;
            TipoTarjeta = "Sin Franquicia";
            Id = $"SUBE-{Guid.NewGuid()}";
        }

        public Tarjeta CargarTarjeta(Tarjeta tarjeta)
        {
            bool cargoTarjeta = false;

            while (!cargoTarjeta)
            {
                Console.WriteLine("¿Cuánto desea cargar?:\n");

                for (int i = 0; i < saldos.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {saldos[i]}");
                }
                Console.Write("\n\nSeleccione una opción: ");

                string opc = Console.ReadLine();

                if (int.TryParse(opc, out int opcion))
                {
                    opcion -= 1; // ajustar al índice real del array

                    if (opcion >= 0 && opcion < saldos.Length)
                    {
                        if (tarjeta.Saldo + saldos[opcion] <= 40000)
                        {
                            cargoTarjeta = true;
                            Console.WriteLine($"Cargaste {saldos[opcion]}");
                            tarjeta.Saldo += saldos[opcion];
                        }
                        else
                        {
                            Console.WriteLine("No se puede cargar, superas el máximo de $40000");
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

            return tarjeta;
        }

        public virtual bool Pagar(double monto)
        {
            if (Saldo + SALDO_NEGATIVO >= monto)
            {
                Saldo -= monto;
                fechaUltimoViaje = DateTime.Now;
                return true;
            }
            return false;
        }

    }

    // ----------------------------------------------------
    // Subclases
    // ----------------------------------------------------

    public class MedioBoletoEstudiantil : Tarjeta
    {
        private int viajesHoy = 0;
        private DateTime fechaUltimoViajeConDescuento = DateTime.MinValue;

        public MedioBoletoEstudiantil()
        {
            TipoTarjeta = "Medio Boleto Estudiantil";
        }

        public override bool Pagar(double monto)
        {
            DateTime ahora = DateTime.Now;
            DateTime hoy = DateTime.Today;
            double montoAPagar;

            // Si cambió el día, reiniciamos el contador
            if (fechaUltimoViaje.Date != hoy)
            {
                viajesHoy = 0;
            }

            // Verificar que hayan pasado al menos 5 minutos desde el último viaje (de cualquier tipo)
            if ((ahora - fechaUltimoViaje).TotalMinutes < 5)
            {
                Console.WriteLine("Debe esperar al menos 5 minutos antes de usar nuevamente la tarjeta.");
                return false;
            }

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

            return base.Pagar(montoAPagar);
        }
    }


    public class BoletoGratuitoEstudiantil : Tarjeta
    {
        public BoletoGratuitoEstudiantil()
        {
            TipoTarjeta = "Boleto Gratuito Estudiantil";
        }
        
        private int viajesHoy = 0;

        public override bool Pagar(double monto)
        {
            DateTime hoy = DateTime.Today;

            if (hoy != fechaUltimoViaje)
            {
                viajesHoy = 0;
            }

            if (viajesHoy < 2)
            {
                viajesHoy++;
                Console.WriteLine($"Viaje gratuito #{viajesHoy} del día ({fechaUltimoViaje.ToShortDateString()})");
                return base.Pagar(0);
            }
            else
            {
                Console.WriteLine("Ya utilizaste los 2 boletos gratuitos del día. No se puede viajar gratis.");
                return base.Pagar(monto);
            }
        }
    }
    public class FranquiciaCompleta : Tarjeta
    {
        public FranquiciaCompleta()
        {
            TipoTarjeta = "Franquicia Completa";
        }
        public override bool Pagar(double monto)
        {
            Console.WriteLine("Viaje gratuito por franquicia completa.");
            return base.Pagar(0);
        }
    }
}