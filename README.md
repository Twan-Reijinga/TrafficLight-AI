# TrafficLightsAI
#About Our PWS
Ons onderzoek is gericht op het optimaliseren van het verkeerslichtsysteem met kunstmatige intelligentie. We hebben hiervoor een verkeerssimulatie gebouwd en aangestuurd met een reinforcement learning-algoritme. Verkeerslichten zijn essentieel in onze maatschappij wereldwijd. Met het toenemende aantal weggebruikers wordt de optimalisatie van verkeerslichten een steeds urgenter vraagstuk. Grote vooruitgangen in technologie en kunstmatige intelligentie bieden mogelijkheden om dit vraagstuk aan te pakken. 

Achter onze huidige verkeerslichten schuilen al ingewikkelde algoritmes die voortdurend berekeningen maken om iedereen zo vlot mogelijk van A naar B te leiden. Maar hoe werken deze algoritmes? Hoe kan kunstmatige intelligentie dit verbeteren? Welke algoritmes zijn het meest geschikt? En welke data heeft kunstmatige intelligentie nodig voor efficiënte doorstroom? Deze vragen vormen de kern van ons uitgebreide onderzoek.

# Installation
### Step 1
```bash
git clone github.com/Twan-Reijinga/TrafficLightsAI
```

### Step 2

Now [Install Untiy](https://store.unity.com/download) and open it <br>
Click on ```Add project from disk``` and select the ```/TrafficLightAI``` directory that you just cloned in step 1.

### Step 3
Open the terminal again and install conda, create and activate an enviorment (with python 3.7) with the following commands:
```bash
pip install conda

conda create -n trafficAI python=3.7.13 anaconda

conda activate trafficAI

pip install gym numpy torch
```

### Step 4
Now go to the cloned ```/TrafficLight``` directory from the terminal that you cloned in step 1.

```bash
python Assets/Scripts/Python/main.py
```

If everything is alright you see an ```Enter port:``` prompt. answer with ```8000``` and press the Enter-key

### Step 5
Open een new Terminal window and again type:
```bash
conda activate trafficAI
```
followed by:
```bash
python Assets/Scripts/Python/main.py
```

But now for the promt ```Enter port:``` answer with ```8001```

### Step 6
Now open the project in Unity and press the play button. The traffic simulation is ready to learn!

TODO - Writing explaination for:
- Switch between ```AI``` and ```Dutch system```
- Save network
- Load network
- Run Gym test enviorment: ```python Assets/Scripts/Python/main.py``` followed by ```Y```

