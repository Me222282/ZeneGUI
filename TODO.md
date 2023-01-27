- Depth control

+ Content scrolling
+ Element that manages zoom and panning

- Resizable elements

- Layouts for automatically organising elements

+ Redo Element.cs, split into multiple simpler objects
    + Layout - Calculates bounds of element
    + LayoutManager - Calculates bounds of element and child elements
    + GUIStruct - Controls LayoutManager and other gui bound based things (like whether mouse is in bounds)
    + GUIManager - Controls focus element and mouse hover element. Also controls keyboard and mouse inputs.
    + GraphicsManager - Controls rendering, rendering bounds, and framebuffer states (like scissor and viewport)
