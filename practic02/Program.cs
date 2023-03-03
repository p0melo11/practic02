using System.Data;
using Newtonsoft.Json;
using HttpContent = System.Net.Http.HttpContent;

namespace Program;

class Pokemon
{
    public string name { get; set; }
    public string url { get; set; }
}

class Response
{
    public Pokemon[] results { get; set; }
}

class Program
{
    public static void Main()
    {
        Console.WriteLine("Hello!");

        var task = GetUsers();
        task.Wait();

        var pokemons = task.Result;

        var pokemonsWithNameStartsWithC = pokemons.ToList().Where(pokemon => pokemon.name.ToLower().StartsWith("c"));

        var orderedByName = pokemons.ToList();
        orderedByName.Sort((pokemonA, pokemonB) => String.Compare(pokemonA.name, pokemonB.name, StringComparison.Ordinal));

        var pokemonsGroupedByNameFirstLetter = pokemons.ToList().GroupBy((pokemon) => pokemon.name.ToLower()[0]);

        Comparison<Pokemon> sortByNameLength = (Pokemon pokemonA, Pokemon pokemonB) => pokemonA.name.Length - pokemonB.name.Length;

        var orderedByNameLength = pokemons.ToList();
        orderedByNameLength.Sort(sortByNameLength);

        Console.ReadKey();
    }

    public static async Task<Pokemon[]> GetUsers()
    {
        try
        {
            string baseUrl = "https://pokeapi.co/api/v2/pokemon";

            using HttpClient client = new HttpClient();
            using HttpResponseMessage res = await client.GetAsync(baseUrl);
            using HttpContent content = res.Content;

            var data = await content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Response>(data);

            if (response == null)
            {
                throw new NoNullAllowedException();
            }

            return response.results;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            return new Pokemon[] { };
        }
    }
}