import cv2

for x in range(1,8):
	vidFile = 'D:\\research\\S4\\GPU\\%d\\video.mp4' % x
	vidcap = cv2.VideoCapture(vidFile)
	count = 0
	success,image = vidcap.read()
	nrows = len(image)
	ncols = len(image[0])
	npix = nrows * ncols
	print npix
	sum = 0
	saveFile = 'D:\\research\\S4\\GPU\\%d\\pixelPower.txt' % x
	file = open("D:\\research\\S4\\GPU\\1\\pixelPower.txt","a+")
	while success:
		print count
		success,image = vidcap.read()
		#cv2.imwrite("G:\\Semionline\\Experiment\\S4\\GPU\\1\\frame%d.jpg" % count, image)
		for r in xrange(1,nrows):
			for c in xrange(1,ncols):
				rc = image[r,c,0]
				gc = image[r,c,1]
				bc = image[r,c,2]
				power = ((0.42*rc)+(0.23*gc)+(-0.042*bc))
				sum += power
				#print sum
				
		avg = sum / npix
		file.write(str(avg))
		file.write('\n')
		sum = 0
		avg = 0
		count += 1

	file.close()
	