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
            if(esInterurbano)
            {
                if (tarjeta.Pagar(TARIFA_BASICA_INTERURBANO, colectivo))
                {
                    boleto = new Boleto(linea, tarjeta);
                    return true;
                }
                return false;
            }
            else
            {
                if (tarjeta.Pagar(TARIFA_BASICA, colectivo))
                {
                    boleto = new Boleto(linea, tarjeta);
                    return true;
                }
                return false;
            }
        }

        public string ObtenerLinea() => linea;
    }
}

