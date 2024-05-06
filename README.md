# WebDavCheck

a tool to check wether or not webclient is active on machines, supports the use of subnets 

an extendation to https://github.com/G0ldenGunSec/GetWebDAVStatus/

usage:
```
.\WebDavCheck.exe -subnet 10.10.10.0/24 [-threads 50] [-verbose]
.\WebDavCheck.exe -ip 10.10.10.5
execute-assembly /path/to/WebDavCheck.exe [args] (its less than 1MB)
```

this tool is compatible with Inline-Execution as it won't call Environment.Exit on fails

```
InlineExecute-Assembly /path/to/WebDavCheck.exe [args]

bofnet_load /path/to/WebDavCheck.exe
bofnet_execute WebDavCheck.Main [args]
```
