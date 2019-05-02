import sys
import pybrain
from pybrain.tools.xml.networkreader   import NetworkReader

net = NetworkReader.readFrom('C:\Python27\myScripts\Trained_NNv4.xml')

x1 = float(sys.argv[1])
x2 = float(sys.argv[2])
x3 = float(sys.argv[3])
x4 = float(sys.argv[4])
x5 = float(sys.argv[5])
x6 = float(sys.argv[6])
x7 = float(sys.argv[7])
x8 = float(sys.argv[8])
x9 = float(sys.argv[9])
x10 = float(sys.argv[10])

a = net.activate((x1,x2,x3,x4,x5,x6,x7,x8,x9,x10))

def numbers_to_strings(argument):
    switcher = {
        0: 1,
        1: 0,
        2: -1,
    }
    return switcher.get(argument, "nothing")

# print a
# print a.argmax()
print numbers_to_strings(a.argmax())
