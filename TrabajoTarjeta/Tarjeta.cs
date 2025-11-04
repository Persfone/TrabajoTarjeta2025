using System;

namespace TrabajoTarjeta
{
    public class Tarjeta
    {
        public double Saldo { get; set; }

        private readonly int[] saldos = { 2000, 3000, 4000, 5000, 10000, 15000, 20000, 25000, 30000 };

        public Tarjeta()
        {
            Saldo = 0;
        }

        public Tarjeta CargarTarjeta(Tarjeta tarjeta)
        {
            bool cargoTarjeta = false;

            while (!cargoTarjeta)
            {
                Console.WriteLine("¿Cuánto desea cargar?:\n");

                for (int i = 0; i < saldos.Length; i++)
                {
                    Console.WriteLine("" + (i + 1) + ". " + saldos[i]);
                }
                Console.Write("\n\nSeleccione una opción: ");

                string opc = Console.ReadLine();

                if (int.TryParse(opc, out int opcion))
                {
                    opcion -= 1; 

                    if (opcion >= 0 && opcion < saldos.Length)
                    {
                        if (tarjeta.Saldo + saldos[opcion] <= 40000)
                        {
                            cargoTarjeta = true;
                            Console.WriteLine("Cargaste " + saldos[opcion]);
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

        public bool Pagar(double monto)
        {
            if (Saldo >= monto)
            {
                Saldo -= monto;
                return true;
            }
            return false;
        }
    }
}
