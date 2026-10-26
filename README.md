## Todo still (if there is a component(s) you want me to take, I'm fine with that)

- Save `music.xml` on exit.
- New Playlist name-entry dialog, reusable for Rename, with validation (no blanks, no duplicate names).
- Add Song: `OpenFileDialog` filtered to `.mp3`, `.m4a`, `.wma`, `.wav`, then `MusicRepo.AddSong`, returning the new song ID so Person A's code can select and highlight it.
- Remove-song confirmation (MessageBox) and "Remove from Playlist" logic.
- Rename and Delete playlist logic
- **Drag and drop:** B writes the drop logic (add the song ID to the end of the playlist via `MusicRepo`). A wires the `MouseMove` and `DragOver` handlers in the main window. Agree on one method signature up front, for example `AddSongToPlaylist(int songId, string playlistName)`.

- TODO: remove above contents when done

# My Tunes
Import music, make playlists, and play songs.

## Authors
### Chasen Oliver (50%)
- **TODO**

### Daniel Steinmetz (50%)
- UI layout
- Playlist ListBox initial population
- Song DataGrid initial population
- Song search filter
- About window
- Play/Stop implementation

## Procrastination Penalty
Did **not** meet requirements

## Additional requirements
- Renaming and deleting playlists
- Play and Stop styling and commands
- Song search filter



