from PyDAQmx import *
import numpy

# Declaration of variable passed by reference
taskHandle = TaskHandle()
read = int32()
data = numpy.zeros((1000,), dtype=numpy.float64)


try:
    # DAQmx Configure Code
    DAQmxCreateTask("",byref(taskHandle))
    DAQmxCreateAIVoltageChan(taskHandle,"cDAQ1Mod1/ai1","",DAQmx_Val_Cfg_Default,-10.0,10.0,DAQmx_Val_Volts,None)
	#DAQmx_Val_ContSamps
	#DAQmx_Val_FiniteSamps
    DAQmxCfgSampClkTiming(taskHandle,"",5000.0,DAQmx_Val_Rising,DAQmx_Val_ContSamps,8)

    # DAQmx Start Code
    DAQmxStartTask(taskHandle)

    for i in xrange(30):
        # DAQmx Read Code
        DAQmxReadAnalogF64(taskHandle,1000,10.0,DAQmx_Val_GroupByChannel,data,1000,byref(read),None)
	#print("data",data[0])
	print("Acquired ",read.value," points and ",data[0])
	
except DAQError as err:
    print "DAQmx Error: %s"%err
finally:
    if taskHandle:
        # DAQmx Stop Code
        DAQmxStopTask(taskHandle)
        DAQmxClearTask(taskHandle)
