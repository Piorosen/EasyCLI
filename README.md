# EasyCLI

### Framework
- .Net 5.0
- Core 3.1

### Installation
```bash
$> git clone https://github.com/Piorosen/EasyCLI.git
$> dotnet add $(target project) reference $(EasyCLI) # [More Detail](https://docs.microsoft.com/ko-kr/dotnet/core/tools/dotnet-add-reference)
```

### Usage Example
```cs
class foo {
  string bar(int a, int b) {
    return (a + b + 50).ToString();
  }
}
var manager = new EasyCLI.EasyCLI();
manager.AddClass(new foo());
var result = manager.Call<string>("foo bar -- a 30 --b 40");
if (result == "120") {
  Console.WriteLine("Done.");
}
```
