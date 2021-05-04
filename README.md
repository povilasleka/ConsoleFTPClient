# ConsoleFTPClient
Developing simple ftp client according to https://tools.ietf.org/pdf/rfc959.pdf

### Client commands
The main goal is the minimum implementation from rfc959. The minimum implementation commands are:
- send [from] [to] - sends a file from path in your system to ftp given path.
- get [from] [to] - receives a file from path in ftp to a path in a local system.
- open [ip] - opens a connection to ftp server.
- bye - ends ftp connection.

