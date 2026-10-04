using System;
using System.Diagnostics;
using System.IO;
using System.Globalization;
class Program
{
    static void Main()
    {
        DateTime dtDataAtual = DateTime.Today;
        int iQtdDias;
        //decimal dMulta = 2.5;

        Console.WriteLine("Digite um Valor: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal dValor))
        {
            Console.WriteLine("Valor inválido!");
            return;
        };

        Console.WriteLine("Digite uma Data (dd/mm/aaaa): ");
        string? sDataInformada = Console.ReadLine();

        if (DateTime.TryParseExact(sDataInformada, "dd/MM/yyyy", CultureInfo.GetCultureInfo("pt-BR"), DateTimeStyles.None, out DateTime dtDataInformada))
        {
            if (dtDataAtual == dtDataInformada)
            {
                Console.WriteLine("Data Informada é a data atual não pode ser.");
                return;
            }
            if (dtDataAtual > dtDataInformada)
            {
                iQtdDias = (dtDataAtual - dtDataInformada).Days;        
            }
            else
            {
                iQtdDias = (dtDataInformada - dtDataAtual).Days;
            }

            decimal dJuros = dValor * (2.5m/100) * iQtdDias;

            Console.WriteLine($"Juros : {dJuros}");
        }
        else
        {
            Console.WriteLine("Não é uma data Válida");
        }

    }
}