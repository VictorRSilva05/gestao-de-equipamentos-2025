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

        app.MapGet("/fabricantes/editar/{id:int}", ExibirFormularioEdicaoFabricantes);
        app.MapPost("/fabricantes/editar/{id:int}", EditarFabricante);

        app.MapGet("/fabricantes/visualizar", VisualizarFabricantes);

        app.MapGet("/fabricantes/excluir/{id:int}", ExibirFormularioExclusaoFabricante);
        app.MapPost("/fabricantes/excluir/{id:int}", ExcluirFabricante);
        app.Run();
    }

    static Task ExibirFormularioExclusaoFabricante(HttpContext context)
    {
        ContextoDados contextoDados = new ContextoDados(true);
        IRepositorioFabricante repositorioFabricante = new RepositorioFabricanteEmArquivo(contextoDados);

        int id = Convert.ToInt32(context.GetRouteValue("id"));

        Fabricante fabricanteSelecionado = repositorioFabricante.SelecionarRegistroPorId(id);
        string conteudo = File.ReadAllText("ModuloFabricante/Html/Excluir.html");

        StringBuilder stringBuilder = new StringBuilder(conteudo);

        stringBuilder.Replace("#id#", id.ToString());
        stringBuilder.Replace("#nome#", fabricanteSelecionado.Nome);

        string conteudoString = stringBuilder.ToString();

        return context.Response.WriteAsync(conteudoString);
    }

    static Task ExcluirFabricante(HttpContext context)
    {
        int id = Convert.ToInt32(context.GetRouteValue("id"));
        ContextoDados contexto = new ContextoDados(true);
        IRepositorioFabricante repositorioFabricante = new RepositorioFabricanteEmArquivo(contexto);

        repositorioFabricante.ExcluirRegistro(id);

        string conteudo = File.ReadAllText("Compartilhado/Html/Notificacao.html");

        StringBuilder stringBuilder = new StringBuilder(conteudo);

        stringBuilder.Replace("#mensagem#", $"O registro foi excluido com sucesso");

        string conteudostring = stringBuilder.ToString();

        return context.Response.WriteAsync(conteudostring);
    }

    static Task EditarFabricante(HttpContext context)
    {
        int id = Convert.ToInt32(context.GetRouteValue("id"));
        ContextoDados contexto = new ContextoDados(true);
        IRepositorioFabricante repositorioFabricante = new RepositorioFabricanteEmArquivo(contexto);

        string nome = context.Request.Form["nome"].ToString();
        string email = context.Request.Form["email"].ToString();
        string telefone = context.Request.Form["telefone"].ToString();

        Fabricante fabricanteAtualizado = new Fabricante(nome, email, telefone);

        repositorioFabricante.EditarRegistro(id,fabricanteAtualizado);

        string conteudo = File.ReadAllText("Compartilhado/Html/Notificacao.html");

        StringBuilder stringBuilder = new StringBuilder(conteudo);

        stringBuilder.Replace("#mensagem#", $"O registro \"{fabricanteAtualizado.Nome}\" foi editado com sucesso");

        string conteudostring = stringBuilder.ToString();

        return context.Response.WriteAsync(conteudostring);
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

        return context.Response.WriteAsync(conteudostring);
    }

    static Task ExibirFormularioCadastroFabricantes(HttpContext context)
    {
        string conteudo = File.ReadAllText("ModuloFabricante/Html/Cadastrar.html");

        return context.Response.WriteAsync(conteudo);
    }

    static Task ExibirFormularioEdicaoFabricantes(HttpContext context)
    {
        ContextoDados contextoDados = new ContextoDados(true);
        IRepositorioFabricante repositorioFabricante = new RepositorioFabricanteEmArquivo(contextoDados);

        int id = Convert.ToInt32(context.GetRouteValue("id"));

        Fabricante fabricanteSelecionado = repositorioFabricante.SelecionarRegistroPorId(id);
        string conteudo = File.ReadAllText("ModuloFabricante/Html/Editar.html");

        StringBuilder stringBuilder = new StringBuilder(conteudo);

        stringBuilder.Replace("#id#", id.ToString());
        stringBuilder.Replace("#nome#", fabricanteSelecionado.Nome);
        stringBuilder.Replace("#email#", fabricanteSelecionado.Email);
        stringBuilder.Replace("#telefone#", fabricanteSelecionado.Telefone);

        string conteudoString = stringBuilder.ToString();

        return context.Response.WriteAsync(conteudoString);
    }
    static Task VisualizarFabricantes(HttpContext context)
    {
        ContextoDados contextoDados = new ContextoDados(true);
        IRepositorioFabricante repositorioFabricante = new RepositorioFabricanteEmArquivo(contextoDados);

        string conteudo = File.ReadAllText("ModuloFabricante/Html/Visualizar.html");

        StringBuilder stringBuilder = new StringBuilder(conteudo);

        List<Fabricante> fabricantes = repositorioFabricante.SelecionarRegistros();

        foreach (Fabricante f in fabricantes)
        {
            string itemLista = $"<li>{f.ToString()} / <a href=\"/fabricantes/editar/{f.Id}\">Editar</a> / <a href=\"/fabricantes/excluir/{f.Id}\">Excluir</a> </li> #fabricante#";

            stringBuilder.Replace("#fabricante#", itemLista);
        }

        stringBuilder.Replace("#fabricante#", "");

        string conteudoString = stringBuilder.ToString();

        return context.Response.WriteAsync(conteudoString);
    }

    static Task PaginaInicial(HttpContext context)
    {
        string conteudo = File.ReadAllText("Compartilhado/Html/PaginaInicial.html");
        return context.Response.WriteAsync(conteudo);
    }
}
