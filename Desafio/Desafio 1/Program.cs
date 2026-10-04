using System;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

public class Vendas
{
    public string vendedor { get; set; }
    public decimal valor { get; set; }
}

public class DadosVendas
{
    public List<Vendas> vendas { get; set; } = new(); 
}

class Program
{
    static void Main()
    {
        string sCaminhoJson = string.Empty;
        string sNmVendedor = string.Empty;
        decimal dTotalValorComissao = 0;
        decimal dComissao = 0;

        Console.Write("Digite o caminho do Json: ");
        sCaminhoJson = Console.ReadLine()?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(sCaminhoJson))
        {
            Console.WriteLine("Caminho inválido");
            return;
        }   
        
        if (!File.Exists(sCaminhoJson))
        {
            Console.WriteLine("Caminho não encontrado");
            return;
        }

        try
        {
            string Json = File.ReadAllText(sCaminhoJson);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            DadosVendas? dados = JsonSerializer.Deserialize<DadosVendas>(Json, options);

            if (dados?.vendas is null || dados.vendas.Count == 0)
            {
                Console.WriteLine("Sem registro");
                return;
            }

            foreach (var v in dados.vendas)
            {
                if (sNmVendedor != v.vendedor)
                {
                    if (!string.IsNullOrEmpty(sNmVendedor))
                    {  
                        Console.WriteLine($"O Vendedor : {sNmVendedor} tem a comissão de {dTotalValorComissao.ToString("C2", new CultureInfo("pt-BR"))}" ); 
                    }

                    dComissao = 0;
                    dTotalValorComissao = 0;
                    sNmVendedor = v.vendedor;
                }

                if (v.valor < 100)
                {
                    dComissao = 0;
                }
                else if (v.valor < 500)
                {
                    dComissao = (v.valor * 1) / 100;
                }
                else
                {
                    dComissao = (v.valor * 5) / 100;
                };

                dTotalValorComissao += dComissao;
            }
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Arquivo não encontrado");
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Erro no Json: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }
}