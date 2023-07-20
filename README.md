# d3b-emu

d3b-emu is a server emulator for Diablo III Beta.

The only supported version of the client is **0.3.0.7447** (Beta Patch 4).

See the [wiki](https://github.com/Crypto137/d3b-emu/wiki) for more details.

## Features

d3b-emu is based on the November 2011 version of mooege with backported and new features.

- Updated opcodes and message definitions according to information discovered later on.
- Updated most GS systems to the state they were in by the end of the beta.
- Backported GameAttribute script compilation.
- Backported most skills.
- Backported basic DRLG implementation.
- Expanded player data saving and loading (WIP).

## Known Issues

- Battle.net implementation is extremely rudimentary and has many issues.
- All game server systems are still heavily work-in-progress.
- Items are not properly saved / loaded.
- Skill implementations are mostly backported from late beta patches and are not accurate to build 7447.

## Setup ##
- Get 0.3.0.7447 client.
- Build the server.
- Copy ClientData.mpq, CoreData.mpq, and base patch files from Data_D3\PC\MPQs\ to d3b-emu\Assets\MPQ\.
- Start the server and wait for it to process MPQs.
- Launch Diablo III.exe with the following arguments: -launch -auroraaddress localhost:1345
- Log in with test@ as your email and any password.
