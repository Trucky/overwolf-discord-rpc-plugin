## Discord Rich Presence Plugin

### Prepare the Discord Application

Login into your Discord Developer account, create a new application. The application name will be the "Playing.." shown on Discord.

Each application has an Application ID, you will use it to initialize the Discord RPC Client.

Upload your assets: every image has a name, called "key" in Discord language. You can use any image as Large or Small image.

The RPC is composed of:
- Details
- State
- Large Image Key
- Large Image Text (shown as tooltip on hover)
- Small Image Key (shown on the bottom right corner of the Large Image)
- Small Image Text (shown as tooltip on hover)

### Compile the project

Open the `DiscordRPCPlugin.csproj` with Visual Studio 2019 (or newer), compile it, and copy `DiscordRPCPlugin.dll, DiscordRPC.dll, and Newtonson.Json.dll` inside your app, for example, a `plugins` directory.

This plugin uses [discord-rpc-csharp](https://github.com/Lachee/discord-rpc-csharp) to communicate with Discord.

### API

The callback style is the same commonly used in the Overwolf API:

```json
{ "success": bool, "status": "success|error" }
```

#### Methods

- `initialize(applicationID, logLevel, callback)`: Initialize the Discord RPC Client
- `onClientReady(callback)`: Fired when the RPC Client is ready, sends the current Discord User
- `onPresenceUpdate(callback)`: Fired when the RPC is updated
- `onClientError(callback)`: Fired when the RPC Client has errors
- `onLogLine(callback)`: Fired when a log line is received
- `updatePresence(details, state, largeImageKey, largeImageText, smallImageKey, smallImageText, showTimestamps, endTime, button1Text, button1Url, button2Text, button2Url, callback)`: Update the Rich Presence
- `updatePresenceWithButtonsArray(details, state, largeImageKey, largeImageText, smallImageKey, smallImageText, showTimestamps, endTime, buttonsJson, callback)`: Update the Rich Presence with buttons
- `dispose(callback)`: Clear the Rich Presence and dispose the connection

### TypeScript Typings

TypeScript typings for the plugin are available in the [types.ts](types.ts) file.

### Use the Plugin

#### Declare the plugin

In `manifest.json`, declare the plugin under the `extra-objects` property:

```json
"DiscordRPCPlugin": {
    "file": "plugins/DiscordRPCPlugin.dll",
    "class": "overwolf.plugins.DiscordRPCPlugin"
}
```

#### Acquire the plugin from JS

```js
const discordRPCPlugin = await pluginService.getPlugin('DiscordRPCPlugin');

discordRPCPlugin.onClientReady.addListener(console.log);
discordRPCPlugin.onPresenceUpdate.addListener(console.log);
discordRPCPlugin.onClientError.addListener(console.error);
discordRPCPlugin.onLogLine.addListener(console.info);

discordRPCPlugin.initialize('YOUR APPLICATION ID', LogLevel.Info, console.log);
```

#### Update the Rich Presence

```js
discordRPCPlugin.updatePresence('My Details', 'My state', 'large_image_key', 'Large image', 'small_image_key', 'Small image', true, 0, 'Button 1', 'https://button1.url', 'Button 2', 'https://button2.url', console.log);
```

```js
const buttonsJson = '[{"label": "Button 1", "url": "https://button1.url"}, {"label": "Button 2", "url": "https://button2.url"}]';
discordRPCPlugin.updatePresenceWithButtonsArray('My Details', 'My state', 'large_image_key', 'Large image', 'small_image_key', 'Small image', true, 0, buttonsJson, console.log);
```

#### Dispose the Rich Presence

```js
discordRPCPlugin.dispose(console.log);
```