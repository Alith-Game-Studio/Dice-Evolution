import os
from PIL import Image

GRAY = 'gray'
RED = 'red'

COLORS = [GRAY, RED]

def main():
    os.chdir('texture')
    filenames = os.listdir()
    inputs = []
    for filename in filenames:
        filename = filename.lower()
        if filename.endswith('.png'):
            for color in COLORS:
                if color in filename:
                    break
            else:
                inputs.append(filename)
    for filename in inputs:
        for color in COLORS:
            img = Image.open(filename).convert('RGBA')
            pixels = img.load()
            for x in range(img.width):
                for y in range(img.height):
                    r, g, b, a = pixels[x, y]
                    if color == GRAY:
                        pixels[x, y] = (
                            r, 
                            g, 
                            b, 
                            round(a * .4), 
                        )
                    elif color == RED:
                        pixels[x, y] = (
                            r, 
                            0, 
                            0, 
                            255, 
                        )
            img.save(filename[:-4] + f'_{color}.png')

main()
