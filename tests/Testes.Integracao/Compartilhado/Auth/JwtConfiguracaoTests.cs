using System.Text.Json;

namespace Testes.Integracao.Compartilhado.Auth;

[TestClass]
public class JwtConfiguracaoTests
{
    [TestMethod]
    public void ConfiguracaoVersionada_NaoContemChaveJwt()
    {
        string raizDoRepositorio = LocalizarRaizDoRepositorio();
        string appsettings = Path.Combine(raizDoRepositorio, "src", "Api", "appsettings.json");

        Assert.IsTrue(File.Exists(appsettings), "src/Api/appsettings.json não foi encontrado.");

        using JsonDocument documento = JsonDocument.Parse(File.ReadAllText(appsettings));

        Assert.IsTrue(
            documento.RootElement.TryGetProperty("Jwt", out JsonElement jwt),
            "A seção Jwt deveria existir em appsettings.json."
        );
        Assert.IsTrue(jwt.TryGetProperty("Issuer", out _));
        Assert.IsTrue(jwt.TryGetProperty("Audience", out _));
        Assert.IsTrue(jwt.TryGetProperty("AccessTokenMinutes", out _));
        Assert.IsFalse(
            jwt.TryGetProperty("Key", out _),
            "Jwt:Key não deve estar versionada em appsettings.json."
        );

        foreach (string arquivo in EnumerarArquivosDeConfiguracao(raizDoRepositorio))
            AssertarAusenciaDeChaveJwt(arquivo);
    }

    private static IEnumerable<string> EnumerarArquivosDeConfiguracao(string raizDoRepositorio)
    {
        return Directory
            .EnumerateFiles(raizDoRepositorio, "*.json", SearchOption.AllDirectories)
            .Where(caminho =>
            {
                string relativo = Path.GetRelativePath(raizDoRepositorio, caminho);
                string nome = Path.GetFileName(caminho);

                if (relativo.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                    .Any(parte => parte is "bin" or "obj" or "TestResults" or ".git" or "studies"))
                    return false;

                return nome.Equals("appsettings.json", StringComparison.OrdinalIgnoreCase)
                    || nome.StartsWith("appsettings.", StringComparison.OrdinalIgnoreCase)
                    || nome.Equals("launchSettings.json", StringComparison.OrdinalIgnoreCase);
            });
    }

    private static void AssertarAusenciaDeChaveJwt(string caminho)
    {
        using JsonDocument documento = JsonDocument.Parse(File.ReadAllText(caminho));

        AssertarAusenciaDeChaveJwt(documento.RootElement, caminho);
    }

    private static void AssertarAusenciaDeChaveJwt(JsonElement elemento, string caminho)
    {
        if (elemento.ValueKind == JsonValueKind.Object)
        {
            foreach (JsonProperty propriedade in elemento.EnumerateObject())
            {
                if (propriedade.NameEquals("Jwt")
                    && propriedade.Value.ValueKind == JsonValueKind.Object
                    && propriedade.Value.TryGetProperty("Key", out _))
                {
                    Assert.Fail($"Jwt:Key não deve estar versionada em {caminho}.");
                }

                if (propriedade.Name.Contains("Jwt:Key", StringComparison.OrdinalIgnoreCase)
                    || propriedade.Name.Equals("Jwt__Key", StringComparison.OrdinalIgnoreCase))
                {
                    Assert.Fail($"A chave JWT não deve estar versionada em {caminho}.");
                }

                AssertarAusenciaDeChaveJwt(propriedade.Value, caminho);
            }
        }
        else if (elemento.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement item in elemento.EnumerateArray())
                AssertarAusenciaDeChaveJwt(item, caminho);
        }
    }

    private static string LocalizarRaizDoRepositorio()
    {
        DirectoryInfo? diretorio = new(AppContext.BaseDirectory);

        while (diretorio is not null)
        {
            if (File.Exists(Path.Combine(diretorio.FullName, "GeradorDeCertificados.slnx"))
                && File.Exists(Path.Combine(diretorio.FullName, "src", "Api", "appsettings.json")))
            {
                return diretorio.FullName;
            }

            diretorio = diretorio.Parent;
        }

        throw new InvalidOperationException(
            "Não foi possível localizar a raiz do repositório a partir do diretório de testes."
        );
    }
}
