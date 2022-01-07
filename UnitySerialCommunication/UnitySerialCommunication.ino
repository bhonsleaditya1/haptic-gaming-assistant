int flexValue[] = {0, 0, 0, 0};
int flexPin[] = {A0, A2, A4, A6};
unsigned long startTime[] = {0, 0, 0, 0};
int jump[] = { -1, -1, -1, -1};
int control[] = {false, false, false, false};
const int downthreshold[] = {800, 800, 800, 790}, threshold[] = {770, 770, 770, 770}, maxThreshold[] = {950, 950, 950, 950}, upthreshold[] = {860, 850, 850, 850};
const int motora[] = {2, 4, 6, 8}, motorb[] = {3, 5, 7, 9};
boolean sendMessage = false;
String data;
unsigned long currentMillis;
int i, j, k, moveTime = 2000;

void setup()
{
  Serial.begin(115200);
  pinMode(2, OUTPUT);
  pinMode(3, OUTPUT);
  pinMode(4, OUTPUT);
  pinMode(5, OUTPUT);
  pinMode(6, OUTPUT);
  pinMode(7, OUTPUT);
  pinMode(8, OUTPUT);
  pinMode(9, OUTPUT);

  i = 0;
  while (i < 4) {
    while (analogRead(flexPin[i]) < threshold[i]) {
      digitalWrite(motora[i], HIGH);
      digitalWrite(motorb[i], LOW);
    }
    i++;
  }
}

void loop()
{
  currentMillis = millis();
  if (Serial.available() > 0)
  {
    String incomming = Serial.readString();
    if (incomming == "Hello")
    {
      Serial.print(incomming);
      Serial.println(" received");
      delay(400);
    }
    if (incomming == "Left") {
      startTime[0] = currentMillis;
      data = "Left";
      control[0] = true;
    }
    else if (incomming == "Right") {
      startTime[1] = currentMillis;
      control[1] = true;
      data = "Right";
    }
    else if (incomming == "Up") {
      startTime[2] = currentMillis;
      control[2] = true;
      data = "Up";
    }
    else if (incomming == "Down") {
      startTime[3] = currentMillis;
      control[3] = true;
      data = "Down";
    }
  }
  i = 0;
  while (i < 4) {
    flexValue[i] = analogRead(flexPin[i]);
    if (currentMillis - startTime[i] <= moveTime) {
      if (control[i]) {
        jump[i] = 0;
      }
    }
    else {
      if (jump[i] == 0) {
        jump[i] = 1;
      }
      if (jump[i] == 1 && flexValue[i] < downthreshold[i]) {
        jump[i] = -1;
        control[i] = false;
        sendMessage = true;
      }
    }
    if (flexValue[i] < threshold[i] && flexValue[i] > maxThreshold[i]) {
      jump[i] = -1;
      control[i] = false;
      while (analogRead(flexPin[i]) < downthreshold[i]) {
        digitalWrite(motora[i], HIGH);
        digitalWrite(motorb[i], LOW);
      }
    }

    switch (jump[i]) {
      case 0: digitalWrite(motora[i], HIGH);
        digitalWrite(motorb[i], LOW);
        //        Serial.print(i);
        //        Serial.print(" ");
        //        Serial.println("UP");
        break;
      case 1:  digitalWrite(motora[i], LOW);
        digitalWrite(motorb[i], HIGH);
        //        Serial.print(i);
        //        Serial.print(" ");
        //        Serial.println("Down");
        break;
      default: digitalWrite(motora[i], LOW);
        digitalWrite(motorb[i], LOW);
        //        Serial.print(i);
        //        Serial.print(" ");
        //        Serial.println("Stop");
        break;
    }
    i++;
  }
  if (sendMessage) {
    //Sending Data Back
    Serial.println(data);
    delay(400);
    sendMessage = false;
  }
  //printSensorValues();
}

void printSensorValues() {
  //For Monitoring Flex Sensor values
  Serial.print("sensor: ");
  for (int i = 0; i < 4; i++) {
    flexValue[i] = analogRead(flexPin[i]);
    Serial.print(flexValue[i]);
    Serial.print(" ");
  }
  Serial.println();
}
