using System;

namespace TrabajoTarjeta
{
    public class Colectivo
    {
        public const double TARIFA_BASICA = 1580;
        private string linea;

        public Colectivo(string linea)
        {
            this.linea = linea;
        }

        public bool PagarCon(Tarjeta tarjeta, out Boleto boleto)
        {
            boleto = null;
            if (tarjeta.Pagar(TARIFA_BASICA))
            {
                boleto = new Boleto(linea, tarjeta);
                return true;
            }
            return false;
        }

        public string ObtenerLinea() => linea;
    }
}

