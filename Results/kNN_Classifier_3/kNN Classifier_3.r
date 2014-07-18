library(rjson)
library(plyr)
library(class)

normalize <- function(x)
{
  return ((x - min(x)) / (max(x) - min(x)))  
}

# Import Training data from JSON file
path <- "C:\\Users\\Eric\\Dropbox\\SVN_FILES\\School Projects\\Chuan Self Study\\Sample Data\\TrainingData.json"
c <- file(path, "r")
l <- readLines(c, -1L)
json <- lapply(X=l, fromJSON)
myList <- list()

for(i in 1:length(json))
{
  myList[[i]] <- data.frame(json[i])
}

TrainingData <- rbind.fill(myList)
TrainingData$Sign <- factor(TrainingData$Sign, levels = c("A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"))

# Import Test data from JSON file
path <- "C:\\Users\\Eric\\Dropbox\\SVN_FILES\\School Projects\\Chuan Self Study\\Sample Data\\TestData.json"
c <- file(path, "r")
l <- readLines(c, -1L)
json <- lapply(X=l, fromJSON)
myList <- list()

for(i in 1:length(json))
{
  myList[[i]] <- data.frame(json[i])
}

TestData <- rbind.fill(myList)
TestData$Sign <- factor(TestData$Sign, levels = c("A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"))

# Copy Labels
Training_Labels <- TrainingData[,length(TrainingData)]
Test_Labels <- TestData[,length(TestData)]

# Remove Label Columns and Normalize
TrainingData <- as.data.frame(lapply(TrainingData[, 1:length(TestData)-1], scale))
TestData <- as.data.frame(lapply(TestData[, 1:length(TestData)-1], scale))

# Run kNN
prediction <- knn(train = TrainingData, test = TestData, cl = Training_Labels, k = 43)

# Summarize Results
crossTable <- table(prediction, Test_Labels)
Results <- 1:length(crossTable[1,])
for(i in 1:length(crossTable[1,]))
{
  s <- sum(crossTable[,i])
  if(s != 0)
  {
    Results[i] <- crossTable[i,i]/s
  }
  else
  {
    Results[i] = 0
  }
}

crossTable <- rbind(crossTable, Results)
overallPerformance <- sum(Results)/ length(Results)