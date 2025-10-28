using System;
using TrabajoTarjeta;
using static TrabajoTarjeta.BoletoGratuitoEstudiantil;

class Program
{
    static void Main()
    {
        var tarjeta = new Tarjeta();
        Colectivo colectivo = null;
        bool salir = false;

        while (!salir)
        {

            Console.Clear();
            Console.WriteLine("Bienvenido al sistema de transporte público\n");
            Console.WriteLine($"Saldo actual de la tarjeta: ${tarjeta.Saldo}");
            Console.WriteLine("\nSeleccione una opción:");
            Console.WriteLine("1. Cargar tarjeta");
            Console.WriteLine("2. Elegir línea");
            Console.WriteLine("3. Pagar boleto");
            Console.WriteLine("4. Salir");
            Console.Write("\nSeleccione una opción: ");
            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    Console.Clear();
                    tarjeta.CargarTarjeta(tarjeta);
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    Console.ReadKey();
                    break;

                case "2":
                    Console.Clear();
                    Console.Write("Ingrese la línea de colectivo (ej. 143): ");
                    string linea = Console.ReadLine();
                    colectivo = new Colectivo(linea);
                    Console.WriteLine($"Línea {linea} seleccionada.");
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    Console.ReadKey();
                    break;

                case "3":
                    Console.Clear();
                    if (colectivo == null)
                    {
                        Console.WriteLine("Primero seleccione una línea (opción 2).");
                    }
                    else
                    {
                        Boleto boleto;
                        bool pagoExitoso = colectivo.PagarCon(tarjeta, out boleto);
                        if (pagoExitoso)
                        {
                            boleto.Imprimir();
                        }
                        else
                        {
                            Console.WriteLine("Saldo insuficiente para pagar el boleto.");
                        }
                    }
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    Console.ReadKey();
                    break;

                case "4":
                    Console.Clear();
                    salir = true;
                    break;

                default:
                    Console.WriteLine("Opción inválida.");
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    Console.ReadKey();
                    break;
            }
        }
    }
}

        /*tarjeta.Cargar(20000);
        var colectivo = new Colectivo("143");
        var boleto = colectivo.PagarCon(tarjeta);
        if (boleto != null)
        {
            boleto.Imprimir();
        }
        else
        {
            Console.WriteLine("Saldo insuficiente");
        }*/
 

