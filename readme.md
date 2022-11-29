# Fake API
This is a fake/dummy api that will return pre-defined values for any route or verb used. 
The idea here is to make it easy to test a application that depends on 3rd party APIs.

Let's say you want to test a new feature in your API. 
However, this API makes a GET request to API-A and a POST request to API-B.
This means you must know the business rules in the black box that is APIs A and B, you need 
dev credentials to those APIs, know the right users to select, etc.

Enter `Fake API`.
You switch the base url of the API you want to mock and this app will catch any route and return whatever 
you want. This means your testing will be way easier, since you can control what the 3rd party APIs respond.

# How to use

```shell
FakeApi.exe -port=5000 -host=http://localhost -statusCode=200 -fileOption=cycle -file=./file1.txt -file=./fil2.txt 
```

- `-port`: Defines the port that the API will use. (Default: 5000)
- `-host`: Host that will be passed to the run function. (Default: https://localhost)
- `-statusCode`: The status code that will be returned for every request. (Default: 200)
- `-fileOption`: If set to `Fixed`, will always return the first file on the list. If set to `Cycle` the this API will go through every file in the list over and over again. `None` means that nothing (body) will be returned. The path to the files can be relative or absolute. If no files (next argument) are passed, this option has no effect. (Default: `cycle`)
- `-file`: For each file, you should inform a `-file=` argument in the command line.

Note that everything that has a default value may be omitted from the command line.

## Minimal CLI arguments:
```shell
FakeAPI.exe
```
This will start Fake API on port `5000` and return `200` for every request.

## Defining custom status code:
```shell
FakeAPI.exe -statusCode 400
```
Almost the same as before, but will return `400` for every request.

## Returning the content of files
For this example, consider the following files:

`boolean-return.txt` (file 1) content:
```text
true
```

`json-return.json` (file 2)  content:
```json
{
  "glossary": {
    "title": "example glossary",
    "GlossDiv": {
      "title": "S",
      "GlossList": {
        "GlossEntry": {
          "ID": "SGML",
          "SortAs": "SGML",
          "GlossTerm": "Standard Generalized Markup Language",
          "Acronym": "SGML",
          "Abbrev": "ISO 8879:1986",
          "GlossDef": {
            "para": "A meta-markup language, used to create markup languages such as DocBook.",
            "GlossSeeAlso": ["GML", "XML"]
          },
          "GlossSee": "markup"
        }
      }
    }
  }
}
```

`boolean-return.txt` (file 3)  content:
```text
The quick brown fox jumps over the lazy dog.
```

All those 3 files are in a folder called `SampleDataFiles`.

```shell
FakeAPI.exe -file=SampleDataFiles/boolean-return.txt -file=.\SampleDataFiles\json-return.json -file=.\SampleDataFiles\text-return.txt
```

With this command, the first request will return the contents of `boolean-return.txt`, the second request will return the contents of `json-return.json`, the third request will return the contents of file `text-return.txt`. The fourth request will go back to the first file and the cycle begins again.

## Returning the content of the first file
```shell
FakeAPI.exe -fileOption=Fixed -file=SampleDataFiles/boolean-return.txt -file=.\SampleDataFiles\json-return.json -file=.\SampleDataFiles\text-return.txt
```

The command above will make the Fake Api always return the contents of the first file. Does it make much sense? I don't know...



# TODO
1. Create config files (instead of relying on command line arguments)
2. Return content based on partial match on endpoint.
3. Increase tests to 100%.