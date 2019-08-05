# Senko.Discord
An unofficial wrapper/API for Discord for .NET Core.

## Fork
This project is a fork of [Miki.Discord](https://github.com/Mikibot/Miki.Discord). The main differences between Miki.Discord and Senko.Discord are the following:

- Senko.Discord targets .NET Core 2.1 instead of .NET Standard 2.0.
- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) has been replaced with [SpanJson](https://github.com/Tornhoof/SpanJson).
- [Miki.Cache](https://github.com/Mikibot/Miki.Cache) has been replaced by [Foundatio](https://github.com/FoundatioFx/Foundatio).
- [Miki.Logging](https://github.com/Mikibot/Miki.Logging) has been replaced by [Microsoft.Extensions.Logging.Abstraction](https://github.com/aspnet/Extensions/tree/master/src/Logging).

## Project structure
- **Senko.Discord**: Implementation of IDiscordClient.  
  References Senko.Discord.Gateway and Senko.Discord.Rest to simplify the NuGet installation.
- **Senko.Discord.Core**: Core classes and interfaces.
- **Senko.Discord.Gateway**: Implementation of IDiscordGateway.
- **Senko.Discord.Rest**: Implementation of IDiscordApiClient.

## Example
See the project [Senko.Discord.Example](examples/Senko.Discord.Example) for a simple example.