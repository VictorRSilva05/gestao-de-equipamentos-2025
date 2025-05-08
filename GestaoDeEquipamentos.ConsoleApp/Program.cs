using GestaoDeEquipamentos.ConsoleApp.Compartilhado;
using GestaoDeEquipamentos.ConsoleApp.ModuloFabricante;
using GestaoDeEquipamentos.ConsoleApp.Util;
using System.Text;

namespace GestaoDeEquipamentos.ConsoleApp;

class Program
{
    static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        WebApplication app = builder.Build();

        app.MapGet("/", PaginaInicial);

        app.MapGet("/fabricantes/cadastrar", ExibirFormularioCadastroFabricantes);
        app.MapPost("/fabricantes/cadastrar", CadastrarFabricante);
        app.MapGet("/fabricantes/visualizar", VisualizarFabricantes);

        app.Run();
    }

    static Task CadastrarFabricante(HttpContext context)
    {
        ContextoDados contexto = new ContextoDados(true);
        IRepositorioFabricante repositorioFabricante = new RepositorioFabricanteEmArquivo(contexto);

        string nome = context.Request.Form["nome"].ToString();
        string email = context.Request.Form["email"].ToString();
        string telefone = context.Request.Form["telefone"].ToString();

        Fabricante fabricante = new Fabricante(nome,email, telefone);

        repositorioFabricante.CadastrarRegistro(fabricante);

        string conteudo = File.ReadAllText("Compartilhado/Html/Notificacao.html");

        StringBuilder stringBuilder = new StringBuilder(conteudo);

        stringBuilder.Replace("#mensagem#", $"O registro \"{fabricante.Nome}\" foi cadastrado com sucesso");

        string conteudostring = stringBuilder.ToString();

        return context.Response.WriteAsync("Fabricante cadastrado!");
    }

    static Task ExibirFormularioCadastroFabricantes(HttpContext context)
    {
        string conteudo = File.ReadAllText("ModuloFabricante/Html/Cadastrar.html");

        return context.Response.WriteAsync(conteudo);
    }

    static Task VisualizarFabricantes(HttpContext context)
    {
        ContextoDados contextoDados = new ContextoDados(true);
        IRepositorioFabricante repositorioFabricante = new RepositorioFabricanteEmArquivo(contextoDados);

        string conteudo = File.ReadAllText("ModuloFabricante/Html/Visualizar.html");

        StringBuilder stringBuilder = new StringBuilder(conteudo);
        foreach (Fabricante fabricante in repositorioFabricante.SelecionarRegistros())
        {
            string stringCompleta = stringBuilder.ToString();
            string itemLista = $"<li>{fabricante.ToString()}</li> #fabricante#";

            stringBuilder.Replace("#fabricante#", itemLista);

        }

        stringBuilder.Replace("#fabricante#", "");

        string conteudoString = stringBuilder.ToString();

        return context.Response.WriteAsync(conteudo);
    }

    static Task PaginaInicial(HttpContext context)
    {
        string conteudo = File.ReadAllText("Html/PaginaInicial.html");
        return context.Response.WriteAsync(conteudo);
    }
}
