# Senko.Discord
An unofficial wrapper/API for Discord for .NET Standard 2.1.

## Fork
This project is a fork of [Miki.Discord](https://github.com/Mikibot/Miki.Discord). The main differences between Miki.Discord and Senko.Discord are the following:

- [Miki.Cache](https://github.com/Mikibot/Miki.Cache) has been replaced by [Foundatio](https://github.com/FoundatioFx/Foundatio).
- [Miki.Logging](https://github.com/Mikibot/Miki.Logging) has been replaced by [Microsoft.Extensions.Logging.Abstraction](https://github.com/aspnet/Extensions/tree/master/src/Logging).

## Project structure
- **Senko.Discord**: Implementation of IDiscordClient.
- **Senko.Discord.Core**: Core classes and interfaces.
- **Senko.Discord.Gateway**: Implementation of IDiscordGateway.
- **Senko.Discord.Rest**: Implementation of IDiscordApiClient.

## Example
See the project [Senko.Discord.Example](examples/Senko.Discord.Example) for a simple example.
