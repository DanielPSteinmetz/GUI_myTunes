# myTunes: Two-Person Task Split

## Timing note

Split the work by file ownership so you rarely edit the same file at once. WPF's biggest conflict risk is `MainWindow.xaml` and `MainWindow.xaml.cs`, so much of this plan is about managing those two files.

Today is Oct 21, and the procrastination-penalty deadline was midnight Oct 19. If you missed it, that is a 5-point deduction, and the repo may also take the 10% daily penalty if modified after the deadline. Check with Dr. McCown on how that applies to you.

## Step 0: Set up together (about 30 minutes)

One person drives, pushes, and the other clones.

1. Create the Visual Studio project and add TagLibSharp via NuGet.
2. Add `MusicRepo.cs`, `Song.cs`, `music.xml` and `music.xsd`; set the XML and XSD to **Copy if newer**.
3. Add a `.gitignore` (the VS template covers `bin/`, `obj/`, `.vs/`).
4. Create empty shells of every file you will need (below) so each person owns files from the start.
5. Agree on a branch rule: work on feature branches, merge small and often into `main`.

## Person A: main window UI, display and playback

Owns `MainWindow.xaml` and `MainWindow.xaml.cs`.

- Window layout: resizable, a Grid with stretching rows and columns.
- ListBox ("All Music" plus playlists) and a read-only DataGrid showing only title, artist, album and genre.
- Load `music.xml` on startup and save it once on exit.
- Selecting a list item filters the grid: all music, or a playlist ordered by position.
- Play and Stop buttons, the context-menu "Play", and the `MediaPlayer` logic.
- Auto-advance to the next song when one finishes.
- This covers the procrastination-penalty requirements (list box plus all music in the grid).

## Person B: dialogs, toolbar actions and playlist management

Owns new files: `AboutWindow.xaml(.cs)`, `PlaylistNameWindow.xaml(.cs)`, plus a helper class for logic.

- About dialog: a real Window that lists both programmers.
- New Playlist name-entry dialog, reusable for Rename, with validation (no blanks, no duplicate names).
- Add Song: `OpenFileDialog` filtered to `.mp3`, `.m4a`, `.wma`, `.wav`, then `MusicRepo.AddSong`, returning the new song ID so Person A's code can select and highlight it.
- Remove-song confirmation (MessageBox) and "Remove from Playlist" logic.
- Toolbar icons and tooltips, with images in an `Images/` folder.
- Rename and Delete playlist logic (extra 5 points).

## Shared work, split cleanly

- **Drag and drop:** B writes the drop logic (add the song ID to the end of the playlist via `MusicRepo`). A wires the `MouseMove` and `DragOver` handlers in the main window. Agree on one method signature up front, for example `AddSongToPlaylist(int songId, string playlistName)`.
- **Search (extra):** B writes a method that takes a string and applies a `DataView.RowFilter` with LIKE. A adds the TextBox and calls that method.
- **Play/Stop styling (extra):** A takes the button style or control template with triggers and the `MediaCommands` bindings, since it lives in the main window.

## Avoiding conflicts in MainWindow.xaml

This is the one real hotspot. In order of preference:

1. **Stub it first.** In Step 0, write the full layout skeleton (toolbar, ListBox, DataGrid, button bar, search box) with empty event handlers named in advance (`AddSong_Click`, `NewPlaylist_Click`, `About_Click`, `Rename_Click`, and so on). Then A fills in the playback and grid handlers, and B fills in only the bodies of the toolbar and context-menu handlers that call into B's classes. Small line-level edits merge fine.
2. **Put logic in separate classes** (such as `PlaylistManager` and `SongImporter`) so the code-behind makes a one-line call.
3. **Pull before you push, every time**, and commit small. If you both need to touch the XAML, tell each other first.
4. **Move styles to `App.xaml`** or a `Styles.xaml` resource dictionary, so A's Play/Stop template stays out of `MainWindow.xaml`.

## Suggested timeline

| Phase | Person A | Person B |
| --- | --- | --- |
| 1 | Layout, load XML, show All Music and playlists in the ListBox | Dialog windows, toolbar icons, helper class stubs |
| 2 | Playlist filtering, play and stop, save on exit | Add song, new playlist, About dialog wired in |
| 3 | Drag and drop handlers, auto-advance, Play/Stop styling | Remove song and remove from playlist, rename and delete |
| 4 | Test and fix bugs | Search filter, README, log bugs as GitHub Issues |

## Git workflow

- `main` always builds. Never push broken code to it.
- Use branches like `feature/playback` and `feature/dialogs`. Merge via pull request, or at least pull `main` before merging.
- Commit often with real messages; the history is graded as evidence of each person's contribution.
- File a GitHub Issue for every known bug (required, or you lose points).
- Add `fmccown` as a collaborator on the private repo.
- Track who did what as you go, since the README needs contributions and percentages.

## README checklist

- [ ] Brief summary of what the program does
- [ ] Whether the procrastination-penalty requirements were met on time
- [ ] Additional requirements implemented
- [ ] What each teammate contributed
- [ ] Percentage of work by each teammate (aim for roughly 50/50, and keep the commit history honest)
