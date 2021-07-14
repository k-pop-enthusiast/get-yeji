# get-yeji
get-yeji is a simple command line tool that downloads pictures from twitter that are in tweets that match the specified filters.
## instalation
* download a win64 binary from [`releases`](https://github.com/k-pop-enthusiast/get-yeji/releases) and copy to your desired directory
* install through [scoop package manager](https://github.com/lukesampson/scoop)
  1. `scoop bucket add yeji https://github.com/k-pop-enthusiast/yeji`
  2. `scoop install get-yeji`
## configuration
by default get-yeji comes with dummy values that might cause a crash, that's why you need to do config before being able to use it. (asuming you installed it already)
1. get `client_key`,`client_key_secret` and `bearer_key` from [Twitter developer portal](https://developer.twitter.com/en/portal/dashboard)
2. decide where to store the download subdirectories(and don't forget to create the subdirectories too, another thing, end the master directory with "\\" so the System.IO doesn't shit itself)
3. decide how long the delay of each cycle is in miliseconds (this goes into the "interval")
4. write the `scope`, this will determine what search term is going to be downloaded where. write in this form: `<mode>|<term>|<subdirectory name>\,<...>`
```csharp
string[] mode = {
"p",
"st"
}
//"p" stands for profile mode, into <term> goes the profile username
//"st" stands for searchTerm mode, into <term> goes search term
```
5. execute command `get-yeji config` and fill in the data from above:
![console screenshot](https://files.catbox.moe/k0yj71.png)
