## Author: Dylan Cuprewich

from PIL import Image


def encrypt(Image, byteArrays):
    pixelPosition = 0
    encodeImageSize(Image, byteArrays)
    for x in range(Image.size[0]):
        for y in range(Image.size[1]):
            if x == 0 and y < 4:
                pass
            else:
                currentPixel = Image.load()[x, y]
                if pixelPosition < len(byteArrays):
                    arrayPosition = pixelPosition//2
                    newPixel = (changeRGBA(currentPixel[0], byteArrays[arrayPosition][pixelPosition % 2*2+0]),
                                changeRGBA(
                                    currentPixel[1], byteArrays[arrayPosition][pixelPosition % 2*2+1]),
                                changeRGBA(
                                    currentPixel[2], byteArrays[arrayPosition][pixelPosition % 2*2+2]),
                                changeRGBA(currentPixel[3], byteArrays[arrayPosition][pixelPosition % 2*2+3]))
                    Image.load()[x, y] = newPixel
                elif pixelPosition < len(byteArrays)+3:
                    newPixel = (changeRGBA(currentPixel[0], 1),
                                changeRGBA(currentPixel[1], 1),
                                changeRGBA(currentPixel[2], 1),
                                changeRGBA(currentPixel[3], 1))
                    Image.load()[x, y] = newPixel


def encodeImageSize(Image, byteArrays):
    characters = len(byteArrays)
    lengthByte = [0]*32
    for i in range(32, -1, -1):
        if characters >= 2**i:
            lengthByte[32-i] = 1
            characters -= 2**i
    # 8 pixels
    for i in range(8):
        currentPixel = Image.load()[0, i]
        r = changeRGBA(currentPixel[0], lengthByte[i*4])
        g = changeRGBA(currentPixel[1], lengthByte[i*4+1])
        b = changeRGBA(currentPixel[2], lengthByte[i*4+2])
        a = changeRGBA(currentPixel[3], lengthByte[i*4+3])
        Image.load()[0, i] = (r, g, b, a)


def changeRGBA(original, bit):
    if original % 2 == 1:
        if bit == 1:
            pass
        elif bit == 0:
            original -= 1
    elif original % 2 == 0:
        original += bit
    return original
