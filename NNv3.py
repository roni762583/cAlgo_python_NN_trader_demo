print 'importing stuff'
import sys
import pybrain
from pybrain.structure                 import RecurrentNetwork, FullConnection, FeedForwardNetwork, LinearLayer, SigmoidLayer, TanhLayer, SoftmaxLayer
from pybrain.structure.modules         import LSTMLayer
from pybrain.datasets                  import SupervisedDataSet, SequentialDataSet
from pybrain.supervised                import RPropMinusTrainer, BackpropTrainer
from pybrain.tools.shortcuts           import buildNetwork
from pybrain.tools.xml.networkwriter   import NetworkWriter
from pybrain.tools.xml.networkreader   import NetworkReader

print 'building NN'
net = buildNetwork(10, 20, 3, bias=True, hiddenclass=SigmoidLayer, outclass=SoftmaxLayer)



# n = RecurrentNetwork()
# ReluLayer input and other layers
# n.addInputModule(LinearLayer(4, name='in'))
# n.addModule(LSTMLayer(5, name='h1'))
# n.addModule(LSTMLayer(3, name='h2'))
# n.addModule(LSTMLayer(2, name='h3'))
# n.addOutputModule(LinearLayer(1, name='out'))

# n.addConnection(FullConnection(n['in'], n['h1'], name='c1'))
# n.addConnection(FullConnection(n['h1'], n['h2'], name='c2'))
# n.addConnection(FullConnection(n['h2'], n['h3'], name='c3'))
# n.addConnection(FullConnection(n['h3'], n['out'], name='c2'))

# n.addRecurrentConnection(FullConnection(n['h1'], n['h1'], name='rc1'))
# n.addRecurrentConnection(FullConnection(n['h2'], n['h2'], name='rc2'))
# n.addRecurrentConnection(FullConnection(n['h3'], n['h3'], name='rc3'))

# net.sortModules()
# n.activate((0.1,0.2,0.3,0.4))

print 'writing network to file'
NetworkWriter.writeToFile(net, 'EURUSDDATA\Test_NN.xml')

print 'building dataset...'
ds= SupervisedDataSet(10, 3)

print 'opening data file and entering for loop'
tf = open("EURUSDDATA\dataForNN.dat",'r')

for line in tf.readlines():
    data = [float(x) for x in line.strip().split(',') if x != '']
    indata =  tuple(data[:10])
    outdata = tuple(data[10:])
    ds.addSample(indata,outdata)
    # print("indata = {}, outdata = {}", indata, outdata)


print(len(ds))

print 'starting training'
# trainer = RPropMinusTrainer(n, dataset=ds)
# trainer = BackpropTrainer(n, dataset=ds)
# trainer.trainUntilConvergence()
# trainer.train()

trainer = BackpropTrainer(net, ds)

train_errors = [] # save errors for plotting later
EPOCHS_PER_CYCLE = 10
CYCLES = 50
EPOCHS = EPOCHS_PER_CYCLE * CYCLES
for i in xrange(CYCLES):
    trainer.trainEpochs(EPOCHS_PER_CYCLE)
    train_errors.append(trainer.testOnData())
    epoch = (i+1) * EPOCHS_PER_CYCLE
#    print("\r epoch {}/{}".format(epoch, EPOCHS), end="")
    print(epoch, EPOCHS, train_errors[-1])
#     #stdout.flush()
print()
print("final error =", train_errors[-1])


# Plot the errors (note that in this simple toy example, we are testing and training on the same dataset, which is of course not what you'd do for a real project!):
# plt.plot(range(0, EPOCHS, EPOCHS_PER_CYCLE), train_errors)
# plt.xlabel('epoch')
# plt.ylabel('error')
# plt.show()

print 'post training, writing NN to file'
NetworkWriter.writeToFile(net, 'EURUSDDATA\Trained_Test_NN.xml')
# net = NetworkReader.readFrom('EURUSD5MDATA\EURUSD5M_LSTM_NN.xml')
