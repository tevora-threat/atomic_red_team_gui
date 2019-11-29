# atomic_red_team_gui
Windows GUI/Execution Engine for Atomic Red Team Atomics

#Atomic Red Team Overview
The Atomic Red Team project is an open source collection of behavior definitions mapping to the MITRE ATT&CK framework. These can be used to generate Indicators of Compromise (IoCs) and to test the ability to detect and respond to them. 
#Tevora Execution Engine 
Tevora developed an execution engine for attack definitions in the Atomic Red Team project that allows for automated running and reporting of the attacks defined therein on Windows systems. The execution engine runs through these attacks and generates a test plan and report based on the ingested and ran Atomics. This allows for running through all Atomics automatically and basic logging of the performed activity. 


Installation
------------

The Tevora Atomic Red Team execution engine consist of two components:

1. The folder of Atomic definitions

2. The Tevora Execution Engine Executable

In order to run the red team simulation, the Execution Engine exe must be opened, and then configured to point at the directory in which the Atomic definitions are stored as enumerated below:

***Installation Steps:***

1. Download the Tevora Red Team Simulation .zip file from the Tevora portal.

2. Select a system to run the tests on.

a. Tevora recommends running the test on a standard newly provisioned employee workstation with normal EDR/AV software installed in detect only mode

 i. If AV is deployed in block mode, it may interfere with being able to run the execution engine, Tevora recommends running in detect mode to see what would have been blocked without causing issues running the test.

b. If possible, Tevora recommends running in a VM, taking a snapshot prior to running the execution engine for easy reversion.

c. Some atomics are destructive, in that they add things such as registry autoruns without cleaning up, thus reverting from a snapshot is an easy way to ensure the test is run on a clean platform each time

d. For this same reason, Tevora recommends not running this tool on production systems unless careful tuning and removal of destructive atomics is done, or if only single or few atomics that are known to not modify the system are run.

3. Extract the zip folder to a location of your choosing.

4. The atomic red team execution engine is now ready to be run.

Execution
---------

1. Open the TevoraRedSim.exe file

2. Select "Load Atomics" from the bottom right context menu

3. Navigate to the unzipped directory and open the atomics folder, or select a customized folder of atomics.

a. At this point the Atomic Red Team Execution Engine GUI should populate with the loaded atomics

4. The red team execution engine will output results of its runs, to select where these will be saved to, select the "Output Directory" output and select a folder

5. *(optional)* If you want to export a plan of what commands are going to be run, select "Export Test Plan" from the bottom right menu. This will be saved to the "Output Directory" location.

6. After reviewing the loaded atomics, and confirming the test plan, click "Execute Atomics"

a. The tests are now executing and the progress bar at the bottom will record whatis currently executing and how many remain.

b. As each test is ran, the execution engine will log the results to a csv file timestamped on when each IOC occurred.

7. Once testing has completed, review the csv of the test log in the output folder.

Review
------

Upon running the automated simulation, a large number of Indicators of Compromise (IoCs) should be generated. Review the results from your SOC, EDR solution, SIEM and other security monitoring tools to understand what was caught and what was not. The CSV output file should contain sufficient detail, such as command performed on what datetime, to cross reference with security alerts or logs.

The key component of this process is identifying areas which were NOT caught by the automated simulation, and drilling down into why. Ultimately, the goal of running this simulation is to identify areas of weakness in the security monitoring program and improve them. Once the areas that did NOT detected were identified, remediation should be performed to ensure they are detected and thte test re-run in the future.

Although the automated red team sim is a great tool to assess the state of current detection capabilities, and to improve them, it is important not to turn the security detection and response program into one tailored only to Atomic Red Team simulations. Periodic manual red teaming and IOC generation should be performed to assess new risks and to address a changing threat posture. Additionally, Tevora recommends periodically reviewing and updating IoCs to keep relevant with current threat actors.
