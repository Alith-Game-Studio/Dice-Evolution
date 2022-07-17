import os
from PIL import Image

GRAY = 'gray'

def main():
    os.chdir('texture')
    filenames = os.listdir()
    inputs = []
    for filename in filenames:
        filename = filename.lower()
        if filename.endswith('.png') and GRAY not in filename:
            inputs.append(filename)
    for filename in inputs:
        img = Image.open(filename).convert('RGBA')
        pixels = img.load()
        for x in range(img.width):
            for y in range(img.height):
                r, g, b, a = pixels[x, y]
                pixels[x, y] = (
                    r, 
                    g, 
                    b, 
					round(a * .4)
                )
        img.save(filename[:-4] + '_gray.png')

main()
