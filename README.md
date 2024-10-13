# Bubble Shooter Game
## Interesting Features
### Txt-file based level generation
To make level creation easier, it's better to have a clear file structure. So i came up with "code blocks". In each block you can specify the one thing, that matters in a level.
For example, you can write the start position of first spawned bubble. Also, you can map chars with bubble types and then "draw" a lavel with this chars in the LEVEL block. 

This approach could allow **generate many levels with AI** and free up time for level designer to complete more complex tasks. 

Moreover, the file's block-driven architecture **could be easily extended** with new blocks.

![The level .txt file](https://github.com/user-attachments/assets/587ff2d8-ffe8-4794-80fc-a0acaf5a0192)

### Bubble grid
Bubble grid is a set of objects of Bubble class. Every bubble has list of its closest neighbours. When the bubble is required to be destroyed, it launches breadth-first search (BFS) algorithm
to get the queue of same bubbles. 

Moreover, if bubble remains unattached to core bubbles, it will be marked with bfs algorithm and then destroyed. In this case, bfs starts with the first spawned 
bubble (the core or first row of bubbles) to reduce time complexity of algorithm.

