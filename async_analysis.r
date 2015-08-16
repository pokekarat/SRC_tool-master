if(!is.element("apcluster", installed.packages()[,1]) )
{
	local({r <- getOption("repos")
		r["CRAN"] <- "http://cran.csie.ntu.edu.tw/"
		options(repos=r)})
	install.packages("apcluster")
}else{ 
	print("apcluster already installed.")
}

require(apcluster)
#Change to path where raw_data_x.txt exists.
train <- read.table("C:\\Users\\pok\\Research\\Experiment\\Dropbox\\experiment\\data\\test1\\3g_3_async_test.txt", header=TRUE, sep= "\t")
trainData <- train

len <- length(trainData[1,])
for(i in c(1:len))
{
	m <- max(trainData[i])
	trainData[i] <- trainData[i]/m
}


# with power 
measure1 <- trainData
similar1 <- expSimMat(measure1, r=2)
cluster1 <- apcluster(similar1)
measureSync <- measure1 

# without power
measure2 <- measure1[,1:length(measure1)-1]
similar2 <- expSimMat(measure2, r=2)
cluster2 <- apcluster(similar2)

list2 <- c(1:length(cluster2@clusters))
list1 <- c(1:length(cluster1@clusters))

asyncTable <- matrix(0, nrow=length(measure1[,1]), ncol=2)

# For each cluster in cluster2
for(i in list2)
{
	#cat("start i=",i,"\n")
	# For each cluster in cluster1
	for( j in list1)
	{
	
		#cat("j ",j,"\n")
		# Verify and save true to another list
		# cat(cluster2@clusters[[i]] %in% cluster1@clusters[[j]])
		verify <- cluster2@clusters[[i]] %in% cluster1@clusters[[j]]
		if(length(verify)==1) next
		
		#cat("verify ",verify,"\n")
		tmp <- tmp2 <- verify[1]
		
		
		#check if all member of tmp are all true or all false.
		for( k in c(2:length(verify)))
		{
			#cat("k ",k,"\n")
			tmp <- tmp && verify[k]  #check all true
			tmp2 <- tmp2 || verify[k] #check all false
		}
		
		# if all true, then cluster i has no asynchronous break.
		if(tmp == TRUE)
		{
			#cat("all true break\n")
			break
			
		}else{
		
			#cat("print 2 \n")
			# if there exist at lest one false 
			if(tmp2 != FALSE)
			{
				aList = list()
				aList[length(list1)+1] <- -1
				#cat("print 3 \n")
				for( m in c(1:length(verify)))
				{	
					#cat("print ",m, verify[m] ,"\n")	
					if(isTRUE(verify[m]))
					{
						# create group and add it into
						#cat("check ",cluster2@clusters[[i]][m],"\n") 
						aList[[j]][length(aList[[j]])+1] <- cluster2@clusters[[i]][m] 
						#cat("1. aList[",j,"]", aList[[j]], "\n")
					}
				}
				
				for( m in c(1:length(verify)))
				{
					if(!isTRUE(verify[m])){
						# check with consecutive cluster1
						
						for( p in c(1:length(cluster1@clusters)))
						{
							#cat("p = ",p,"\n")
							for( pp in c(1:length(cluster1@clusters[[p]])))
							{
								if(cluster2@clusters[[i]][m] == cluster1@clusters[[p]][pp])
								{
									aList[[p]][length(aList[[p]])+1] <- cluster2@clusters[[i]][m]
									#cat("2. aList[",p,"]", aList[[p]], "\n")
									break;
								}
							}
						}
					}
				}
				
				#cat("end ************************** \n")
				#cat("current cluster",i,"\n")
				
				powerList <- list()
				
				#process average energy
				#rows
				for(a in c(1:length(aList)))
				{
					if(a == length(aList)) break
					#cat("a ",a,"\n")
					#columns
					powerSum <- 0
					
					if(length(aList[[a]]) == 0) next
					
					for(b in c(1:length(aList[[a]])))
					{	
						#cat("b ",b,"\n")
						power <- train[aList[[a]][b],length(measure1)]
						powerSum <- powerSum + power
						#cat(powerSum,"\n")
					}
					
					avgPower <- powerSum/length(aList[[a]])
					#cat("avgPower ",avgPower,"\n")
					
					powerList[a] <- avgPower
					
				}
				
				#cat("Modify power \n")
				minPower <- min(unlist(powerList))
				#cat("minPower ", minPower, "\n")
				for(r in unlist(aList))
				{
					if(r == -1) break
					p1 <- train[r, length(measure1[1,])]
					
					if(p1 > minPower)
					{
						
						asynPower <- (p1 - minPower)
						cat("r=",r,", powDiff=",asynPower," p1=",p1,"minPower",minPower,"\n")
						asyncTable[r,1] <- r
						asyncTable[r,2] <- asynPower
						#cat("asyncTable",asyncTable,"\n")
						measureSync[r, length(measure1[1,])] <- minPower
					}	
				}
				break;
			}
		}
	}
	#cat("finish")
	#cat("\n")
}
#modify train power
trainModify <- train
w <- which(asyncTable[,1] > 0)
trainModify[w,]$power <- train[w,]$power - asyncTable[w,2]
write.table(trainModify,file = "C:\\Users\\pok\\Research\\Experiment\\Dropbox\\experiment\\data\\test1\\trainModify.txt", sep = " ", row.names = F)

asyncTable[w,2]<-train[w,]$power
asyncTableFile <- train[w,]
asyncTableFile$power <- asyncTable[w,2]
write.table(asyncTableFile,file = "C:\\Users\\pok\\Research\\Experiment\\Dropbox\\experiment\\data\\test1\\asyncTable.txt", sep = " ", row.names = F)
