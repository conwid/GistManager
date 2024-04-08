# Design

<!--TOC-->
  - [Icons](#icons)
    - [Gimp Icon manipulations](#gimp-icon-manipulations)
      - [General white items](#general-white-items)
      - [Light colored graphical elements](#light-colored-graphical-elements)
<!--/TOC-->

## Icons

### Gimp Icon manipulations

Gimp Files are included in this documentation. 

To make icons work for both light and dark mode:

#### General white items

Need changing to gray top work in light mode. Select the item - then to Colors>Brightness. Adjust bright slider all way down. Think it's #707070

#### Light colored graphical elements

E.g. the yellow star on 'new' items

Resize canvas to 40x40

Outline any light colored composite items and paste as new layer and get rid of original. Resize Canvas to 40. Layer>Stroke for the light colored element:

Size=4

Pos = 75

Color = #707070

Create own layer group with this in. 

Resize canvas back to 32,32

Move the star (or whatever) further in. Consider deleting some of he bottom layer to provide a gap between the star and the bottom element (See AddGist.png.)