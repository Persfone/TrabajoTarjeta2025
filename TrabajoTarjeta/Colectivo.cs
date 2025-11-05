// TrabajoTarjeta/Colectivo.cs
using System;

namespace TrabajoTarjeta
{
    public class Colectivo
    {
        public const double TARIFA_BASICA = 1580;
        public const double TARIFA_BASICA_INTERURBANO = 3000;
        private string linea;
        private bool esInterurbano;

        public Colectivo(string linea, bool esInterurbano)
        {
            this.linea = linea;
            this.esInterurbano = esInterurbano;
        }

        public bool PagarCon(Tarjeta tarjeta, Colectivo colectivo, out Boleto boleto)
        {
            boleto = null;
            double tarifa = esInterurbano ? TARIFA_BASICA_INTERURBANO : TARIFA_BASICA;

            if (tarjeta.Pagar(tarifa, colectivo))
            {
                boleto = new Boleto(linea, tarjeta);
                return true;
            }
            return false;
        }

        public string ObtenerLinea() => linea;
    }
}