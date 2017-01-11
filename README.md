# ResourceTracker

An enhanced fork of [memoryprofiler](https://bitbucket.org/Unity-Technologies/memoryprofiler) with diff and searching supported.

## Additional features (with all original memoryprofiler functionalities included)

- **Snapshot-Diff** 
  + Multiple snapshots comparison
 
![1](/_img/1.png)

- **Text Searching** 
  + full text searching among all memory objects 
  + results are grouped by type

![2](/_img/2.png)

- **Alloc Tracking** 
  + The source-code allocation point backtracking of a given memory block

![3](/_img/3.png)

- **Minimum Sample** 
  + Contains a sample project (a simulation scene with minimum and recommended usage)

![4](/_img/4.png)


## Changes

- :paperclip: [v1.1.1 released](https://github.com/PerfAssist/ResourceTracker/releases/tag/v1.1.1) [2016-11-25]
    + merged back several fixes
        * resources moved into editor specific folder (4fdf39feabbfeb7a0c10158f31e575ca71f6670c)
        * fix the selection in search result list (5017afe200c12bb5bef65f1f6b5c967800f8d483)
- :star: [v1.1.0 released](https://github.com/PerfAssist/ResourceTracker/releases/tag/v1.1.0) [2016-11-25]
    + :triangular_flag_on_post: add memory object browser (two-lists table view)
        * :small_orange_diamond: add realtime text searching for mem-object browser (7134431409c0f962dd4e07de94c71af6df15bc4f)
        * :small_orange_diamond: add navigation buttons (back & forward) (754d4a3f57abac015db65c4effe350d2188772c1)
        * :small_orange_diamond: add loading progress indicators (2ee1ec5f8b35d828a20c53d1bf958a380bb2f292)



