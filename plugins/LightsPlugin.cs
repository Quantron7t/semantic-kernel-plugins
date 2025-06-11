using System.ComponentModel;
using System.Text.Json.Serialization;
using Microsoft.SemanticKernel;

public class LightsPlugin
{
   // Mock data for the lights
   private readonly List<LightModel> lights = new()
   {
       new LightModel { Id = 1, Name = "Gate lamps", IsOn = false },
       new LightModel { Id = 2, Name = "Porch light", IsOn = false },
       new LightModel { Id = 3, Name = "Chandelier", IsOn = true }
   };

   [KernelFunction("get_lights")]
   [Description("Gets a list of lights and their current state")]
   [return: Description("An array of lights")]
   public Task<List<LightModel>> GetLightsAsync()
   {
       return Task.FromResult(lights);
   }

   [KernelFunction("change_state")]
   [Description("Changes the state of the light")]
   [return: Description("The updated state of the light; will return null if the light does not exist")]
   public Task<LightModel?> ChangeStateAsync(
       [Description("This is the identifier of the light.")]
       int id,
       [Description("True if the light is on; false if the light is off.")]
       bool isOn)
   {
       var light = lights.FirstOrDefault(light => light.Id == id);
       if (light is null)
       {
           return Task.FromResult((LightModel?)null);
       }

       light.IsOn = isOn;
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
       return Task.FromResult(light);
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
   }
}

public class LightModel
{
   [JsonPropertyName("id")]
   public int Id { get; set; }

   [JsonPropertyName("name")]
   public required string Name { get; set; }

   [JsonPropertyName("is_on")]
   public bool? IsOn { get; set; }
}