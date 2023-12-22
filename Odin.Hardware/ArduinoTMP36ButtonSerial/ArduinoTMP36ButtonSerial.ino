const int TMP36_PIN = A0;
const int SWITCH_PIN = 8;
const int DEBOUNCE_DELAY_MS = 50;
const int SERIAL_BAUD_RATE = 9600;

int switchState = LOW;
int lastSwitchState = LOW;
unsigned long lastDebounceTime = 0;
float totalCelsius = 0;
int count = 0;

float readDegreesCelsius();
void sampleSwitch();

void setup() {
  Serial.begin(SERIAL_BAUD_RATE);
  pinMode(TMP36_PIN, INPUT);
  pinMode(SWITCH_PIN, INPUT);
}

void loop() {
  switchState = digitalRead(SWITCH_PIN);

  if (switchState != lastSwitchState) {
    lastDebounceTime = millis();
  }

  if ((millis() - lastDebounceTime) >= DEBOUNCE_DELAY_MS) {
    // Sample the switch state if we've waited the debounced period from the last time pressed
    sampleSwitch();
  }

  lastSwitchState = switchState;
}

float readDegreesCelsius() {
  int adcValue = analogRead(TMP36_PIN);
  float volts = 5.0 * adcValue / 1024;
  return (100 * volts) - 50;
}

void sampleSwitch() {
  if (switchState == HIGH) {
    // Button is pressed
    totalCelsius += readDegreesCelsius();
    count++;
  } else if (count != 0) {
    // Button is released
    Serial.println(totalCelsius / count);
    totalCelsius = 0;
    count = 0;
  }
}