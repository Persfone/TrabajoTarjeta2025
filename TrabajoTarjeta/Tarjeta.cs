using System;

namespace TrabajoTarjeta
{
    public class Tarjeta
    {
        public double Saldo { get; set; }
        public string TipoTarjeta { get; set; }
        public string Id { get; set; }
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
            if (Saldo >= monto)
            {
                Saldo -= monto;
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
        public MedioBoletoEstudiantil()
        {
            TipoTarjeta = "Medio Boleto Estudiantil";
        }
        public override bool Pagar(double monto)
        {
            double montoConDescuento = monto * 0.50;
            return base.Pagar(montoConDescuento);
        }
    }

    public class BoletoGratuitoEstudiantil : Tarjeta
    {
        public BoletoGratuitoEstudiantil()
        {
            TipoTarjeta = "Boleto Gratuito Estudiantil";
        }
        
        private int viajesHoy = 0;
        private DateTime fechaUltimoViaje = DateTime.MinValue;

        public override bool Pagar(double monto)
        {
            DateTime hoy = DateTime.Today;

            // Si cambió el día, se reinicia el contador
            if (hoy != fechaUltimoViaje)
            {
                viajesHoy = 0;
                fechaUltimoViaje = hoy;
            }

            if (viajesHoy < 2)
            {
                viajesHoy++;
                Console.WriteLine($"Viaje gratuito #{viajesHoy} del día ({fechaUltimoViaje.ToShortDateString()})");
                return true;
            }
            else
            {
                Console.WriteLine("Ya utilizaste los 2 boletos gratuitos del día. No se puede viajar gratis.");
                if (Saldo + SALDO_NEGATIVO >= monto)
                {
                    Saldo -= monto;
                    return true;
                }
                return false;
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
            return true;
        }
    }
}