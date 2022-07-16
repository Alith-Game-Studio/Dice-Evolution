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
        img = Image.open(filename).convert('RGB')
        pixels = img.load()
        for x in range(img.width):
            for y in range(img.height):
                r, g, b = pixels[x, y]
                pixels[x, y] = (
                    round(r * .5), 
                    round(g * .5), 
                    round(b * .5), 
                )
        img.save(filename[:-4] + '_gray.png')

main()
