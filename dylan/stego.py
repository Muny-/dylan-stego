## Author: Dylan Cuprewich

from PIL import Image
from encryptImageWithText import encrypt


def main():
    # message limit: each character is 3 pixels, we're using diagonals so 1 for every 2 pixels, so character limit of message is
    # total amount of pixels in picture/3/2

    #Read image
    # fileName = str(raw_input("Please provide image name to hide a message in(include extension):\n"))
    # im = Image.open( '/home/dc/Projects/Stegano/imagemanupilation/'+fileName)
    # print("Message limit is "+str(im.size[0]*im.size[1]/6)+" characters")
    # isInputMessageAFile = str(raw_input("Is message in a file?:\n"))
    # if isInputMessageAFile.lower()=='no':
    #     message = str(raw_input("Enter message to be hidden:\n"))
    # elif isInputMessageAFile.lower()=='yes':
    #     fileName= str(raw_input("Enter file name (assumed to be a .txt file, if it isn't .txt contact creator):"))
    #     openFile = file( fileName, 'r')
    #     message = processFile(openFile)
    # else:
    #     print("Invalid input")
    #     return
    im = Image.open('/home/dc/Projects/Stegano/imagemanupilation/testPNG.png')
    message = "Hello Kevin. Do you have any ideas of what to do with this? Just make it more efficient i guess."
    #Display image
    im.show()
    print(stringToByte(message))
    encrypt(im, stringToByte(message))

    im.save('altered_image.png')  # saves new image with hidden message
    im.show()


"""checks if an integer would be a valid unicode character"""


def isBetween(checkInt):
    characterValueLimits = (32, 126)
    return (checkInt >= characterValueLimits[0]) and (checkInt <= characterValueLimits[1])


"""process a file and returns all text in file as a single string"""


def processFile(openFile):
    message = ''
    for line in openFile:
        message += line.strip()+" "
    message.strip()
    return message


"""returns the bytearray of a string"""


def stringToByte(message):
    byteArray = []
    for char in message:
        charValue = ord(char)
        byteArray.append(charToByte(char))
    return byteArray


"""return bytearray of a character"""


def charToByte(charValue):
    byte = [0, 0, 0, 0, 0, 0, 0, 0]
    value = ord(charValue)
    for i in range(7, -1, -1):
        if value >= 2**i:
            byte[7-i] = 1
            value -= 2**i
    return byte


main()
