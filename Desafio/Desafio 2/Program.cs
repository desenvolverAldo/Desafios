using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Globalization;

public class Produtos
{
    public int codigoProduto {  get; set; }
    public string descricaoProduto { get; set; }
    public int estoque {  get; set; }
}

public class DadosProdutos
{
    public List<Produtos> Estoque { get; set; } = new();
}

public class MovimentacaoProdutos
{
    public int codigo { get; set; }
    public int codigoProduto { get; set; }
    public string tipo { get; set; }
    public string descricaoProduto { get; set; }
    public int qtd {  get; set; }
    public DateTime dataHora { get; set; }
}

class Program
{
    static List<MovimentacaoProdutos> movimentacoes = new();
    static DadosProdutos? dadosProd;
    static int proxCod = 1;
    
    static void Main()
    {
        string sCaminhoJson;

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
            string sJson = File.ReadAllText(sCaminhoJson);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            dadosProd = JsonSerializer.Deserialize<DadosProdutos>(sJson, options);

            if (dadosProd?.Estoque is null || dadosProd.Estoque.Count == 0)
            {
                Console.WriteLine("Sem registro");
                return;
            }
            
            while (true)
            {
                Console.Clear();

                Console.WriteLine("CONTROLE DE ESTOQUE");
                Console.WriteLine("");

                MostrarEstoque();

                Console.WriteLine("");
                Console.WriteLine("Opção :");
                Console.WriteLine("0 - Sair");
                Console.WriteLine("1 - Lançar Entrada do Produto");
                Console.WriteLine("2 - Lançar Saida do Produto");
                Console.WriteLine("3 - Ver Estoque");
                Console.WriteLine("4 - Salvar arquivo (Json");
                Console.WriteLine("");
                Console.WriteLine("Ditite o numero que deseja fazer:");
                
                string? opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "0" : 
                          return;
                    case "1":
                        Lancar("Entrada");
                        break;
                    case "2":
                        Lancar("Saida");
                        break;
                    case "3":
                        VerEstoque();
                        break;
                    case "4":
                        SalvarJson(sCaminhoJson);
                        break;
                    default: 
                        Console.WriteLine("Opção inválida, digite o número (a opção) a desejar");
                        break;

                }
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
    static void MostrarEstoque()
    {
        Console.WriteLine("CODIGO | PRODUTO                       | ESTOQUE ");
        Console.WriteLine("=================================================");

        foreach(var p in dadosProd!.Estoque)
        {
            Console.WriteLine($"{p.codigoProduto, 6} | {p.descricaoProduto, -28}  |  {p.estoque,6}");
        }
    }
    static void Lancar(string TipoLancar)
    {
        Console.WriteLine("Digite o Código do Produto: ");
        if (!int.TryParse(Console.ReadLine(), out int codigo))
        {
            Console.WriteLine("Código inválido!");
            return;
        }

        var produtos = dadosProd!.Estoque.FirstOrDefault(p => p.codigoProduto == codigo);
        if (produtos == null)
        {
            Console.WriteLine("Porduto não encontrado");
            return;
        }

        Console.WriteLine("Digite a Quantidade: ");
        if (!int.TryParse(Console.ReadLine(), out int quantidade))
        {
            Console.WriteLine("Valor da quantidade inválido, só pode ser inteiro!");
            return;
        }

        if ((TipoLancar == "Saida") && (quantidade > produtos.estoque))
        {
            Console.WriteLine("Estoque insuficiente para Saida.");
            return;
        }

        if (TipoLancar == "Saida")
        {
            produtos.estoque += quantidade; 
        }
        else
        {
            produtos.estoque -= quantidade;
        }

        //gravar movimentação
        var movimentacao = new MovimentacaoProdutos
        {
            codigo = proxCod,
            codigoProduto = produtos.codigoProduto,
            tipo = TipoLancar,
            descricaoProduto = TipoLancar == "Entrada" ? "Entrada de mercadoria" : "Saída de mercadoria",
            qtd = quantidade,
            dataHora = DateTime.Now
        };
        movimentacoes.Add(movimentacao);
    }

    static void VerEstoque()
    {
        Console.WriteLine("");
        
        if (movimentacoes.Count == 0)
        {
            Console.WriteLine("SEM MOVIMENTAÇÃO");
        }
        else
        {
            foreach (var mov in movimentacoes)
            {
                var produto = dadosProd!.Estoque.First(p => p.codigoProduto == mov.codigoProduto);
                Console.WriteLine($"Código: {mov.codigo} | {mov.dataHora:dd/MM/yyyy HH:mm} | {mov.tipo,-7} | {produto.descricaoProduto,-25} | Qtd: {mov.qtd}");
            }
        }
    }
    static void SalvarJson(string sCaminho)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            string sJson = JsonSerializer.Serialize(dadosProd, options);
            File.WriteAllText(sCaminho, sJson);

            Console.WriteLine("Salvo com sucesso o aqruivo Json, no mesmo caminho que abriu o arquivo");

        }
        catch (Exception ex) 
        {
            Console.WriteLine($"Error ao salvar: {ex.Message}");

        }
    }
}