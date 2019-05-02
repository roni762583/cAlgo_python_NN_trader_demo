# cAlgo
C# code for cAlgo / cTrader Automated Trading Platform

Proof of concept to use neural networks to predict profitable market moves implemented for the cTrader/cAlgo platform.
A simple feed forward NN was trained on features that are based on prices, to predict market moves above a threshold in the NNv3.py file. 
The trained network saved as xml in file Trained_NNv4.xml
The cTrader / cAlgo code NN_call_v1.cs, calls the python script ReadAndUseNNv4.py with the latest price features, which in turn returns a prediction on if and what direction to trade. The trade is executed on the cTrader platform.

for write up see www.PeoplesFinTech/blog/Neural_Network_Trade_Recommender_Proof_of_Concept
