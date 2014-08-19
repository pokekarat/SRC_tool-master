import sys
import cv2
import datetime

#for x in range(1,4):
x = int(sys.argv[1])
vidFile = 'D:\\research\\S4\\GPU\\%d\\video3.mp4' % x
print vidFile
vidcap = cv2.VideoCapture(vidFile)
#totlFrame = cv.GetCaptureProperty(vidcap, cv.CV_CAP_PROP_FRAME_COUNT)
count = 0
success,image = vidcap.read()
nrows = len(image)
ncols = len(image[0])
npix = nrows * ncols
print("rows x cols",nrows," ",ncols)
sum = 0
saveFile = 'D:\\research\\S4\\GPU\\%d\\screenPower.txt' % x
file = open(saveFile,"w+")
file.write("power")
file.write('\n')
frame = 0
avgPower = 0
while success:
	#print count
	
	#cv2.imwrite("G:\\Semionline\\Experiment\\S4\\GPU\\1\\frame%d.jpg" % count, image)
	#begin = 'frame'+ str(frame)
	#file.write(begin)
	#file.write('\n')
	color = ""
	for r in xrange(1,nrows,6):
		for c in xrange(1,ncols,6):
			try:
				rc = image[r,c,0]
				gc = image[r,c,1]
				bc = image[r,c,2]
			except TypeError:
				rc = -1
				gc = -1
				bc = -1
				
			color = str(rc)+' '+str(gc)+' '+str(bc)
			
			#print(color)
			#file.write(str(color))
			#file.write('\n')
			#lm
			#power = (0.74*255)+(0.42*rc)+(0.23*gc)+(-0.042*bc)-57.67
			
			#quadratic
			power = 217.37+(0.43 * rc)+(0.23 * gc)+(-0.033 * bc) - 46.93			
			sum += power
			#print sum
	print str(sum/2840)
	file.write(str(sum/2840))
	#end = 'end'+ str(frame)
	#file.write(end)
	file.write('\n')
	sum = 0
	count += 1
	while count % 7 != 0:
		success,image = vidcap.read()
		count += 1
		#print count
		continue
	
	frame += 1
	print frame

file.close()
	