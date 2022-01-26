# Introduction
Genguid is a glorified extensible GUID generator incorporating logging functionality.

# Getting Started
Clone the repository in Visual Studio and build the solution to produce executables. There are two Windows-only executables which can be used to launch Genguid:

- `Genguid.Launcher.exe` - Launches a WPF UI. New GUIDs can be generated using the **Next** button, and previously generated GUIDs can be retrieved using the **Previous** button.
- `Genguid.Clipboard.exe` - Generates a single GUID in 'headless' mode (no UI), writes the GUID to the Windows clipboard (as well as the configured log sink), then terminates.

# Configuration
The app can be configured declaratively (via a config file) or imperatively (via code).

## Declarative Configuration
Each executable has its own `App.config` file, which are independent. The `App.config` allows you to specify:

1. The factory used for generating GUIDs.
2. A series observers to be notified whenever a new GUID is generated.
3. A series of formatters which determine the style of GUID that will be generated.
4. The log sink that will be used to store generated GUIDs.

### Factories
There are two available factories:

- `StandardGuidFactory` - For generating pseudo-random GUIDs.
- `CombGuidFactory` - For generating sequential GUIDs using an underlying Windows API.

These are configured within the `App.config` as follows:

- `<guidFactory name="StandardGuidFactory" type="Genguid.Factories.StandardGuidFactory, Genguid"/>`
- `<guidFactory name="CombGuidFactory" type="Genguid.Factories.CombGuidFactory, Genguid"/>`

### Factory Observers
Factory observers are classes that implement the `IGuidFactoryObserver` interface. The following example illustrates how observers can be declared within the `App.config`:

```
<guidFactoryObservers>
  <clear />
  <add name="CustomObserver" type="Genguid.Extensions.CustomObserver, Genguid.Extensions"/>
</guidFactoryObservers>
```

Each time the configured factory generates a new GUID, each configured observer will be notified in the specified order via a call to its `NotifyOfGeneratedGuid` method.

### Formatters
Formatters are classes that extend either the `GuidFormatter` (base formatters) or the `GuidFormatterDecorator` (decorator formatters) abstract class. There is one available base formatter, `CompactGuidFormatter`, and five decorators:

- `BracedGuidFormatter` - For wrapping generated GUIDs in curly braces.
- `ParenthesisedGuidFormatter` - For wrapping generated GUIDs in parentheses.
- `HyphenatedGuidFormatter` - For separating generated GUIDs into five groups separated by hyphens in the form 8-4-4-4-12.
- `UpperCaseGuidFormatter` - For converting alpha characters within generated GUIDs to upper case.
- `LowerCaseGuidFormatter` - For converting alpha characters within generated GUIDs to lower case.

Think of formatters as a pipeline through which GUIDs will be fed and consider the order in which you configure them, for example:

```
<guidFormatters>
  <clear/>
  <add name="CompactGuidFormatter" type="Genguid.Formatters.CompactGuidFormatter, Genguid"/>
  <add name="HyphenatedGuidFormatter" type="Genguid.Formatters.HyphenatedGuidFormatter, Genguid"/>
  <add name="BracedGuidFormatter" type="Genguid.Formatters.BracedGuidFormatter, Genguid"/>
  <add name="LowerCaseGuidFormatter" type="Genguid.Formatters.LowerCaseGuidFormatter, Genguid"/>
</guidFormatters>	
```



