from nltk.corpus import stopwords
from nltk.tokenize import word_tokenize, sent_tokenize

filetoread = open("C:\\Users\\saagraw\\Desktop\\satyanadella.txt", "r") 
text =  filetoread.read() 

#text = "Hello , I am sudipta sen and I will be using NLP to extract highlights. My friends are Neha and Saloni. They are working with me."

stopWords = set(stopwords.words("english"))
words = word_tokenize(text)
l = ['education','technology','batman']
freqTable = dict()
for word in words:
        word = word.lower()
        print(word)
        if word in stopWords:
            continue
        elif word in freqTable: 
            freqTable[word] += 1
        else:
            freqTable[word] = 1

sentences = sent_tokenize(text)
sentenceValue = dict()



for sentence in sentences:
     for index, wordValue in enumerate(freqTable, start=1):
          if wordValue in sentence.lower(): # index[0] return word
               if sentence in sentenceValue:  
                    print(sentence)
                    print(index)
                    if any(word in sentence.lower() for word in l): 
                        sentenceValue[sentence] += 180
                    sentenceValue[sentence] += index 
                    # index return value of occurence of that word
                    #sentenceValue.update({sentence: index})
               else:
                   # sentenceValue[sentence] = wordValue
                   sentenceValue[sentence] = index
                   
                    

sumValues = 0
for sentence in sentenceValue:
    sumValues += sentenceValue[sentence]

# Average value of a sentence from original text
average = int(sumValues/ len(sentenceValue))

summary = ''
for sentence in sentences:
        if sentence in sentenceValue and sentenceValue[sentence] > (1.5 * average):
            summary +=  " " + sentence
print("Summary Below :")
print(summary)
#f= open("C:\\Users\\saagraw\\Desktop\\summary.txt","w+")
f= open(".\summary.txt","w+")
f.write(summary)
f.close()