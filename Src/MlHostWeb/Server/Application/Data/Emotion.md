### Emotion

Emotion NLP model is designed help understand the user's emotional signals.

Emotion detection is the spiritual successor to sentiment analysis.

#### General Emotional Models
Emotion detection models detect presents or absences of several primary emotion signals.
These signals are typical chosen to be...

1) Anger
2) Anticipation
3) Disgust
4) Fear
4) Joy
5) Sadness
6) Trust
7) Surprise


This typical model works well on things like movie reviews, but fairs poorly in the
insurance world, due to domain specific context and terminology.


#### Fine Tuned Model for Premera

What we have done with this model, that we are presenting with this API, is an
attempt to train a model which ranges the span of emotions present in
Premera NLP data.

The categories for this model are...

1) sad: tired - bored - lonely - depressed - ashamed - dispirited - other
2) mad: - frustrated - hurt - hostile - angry - selfish - critical - other
3) upset: - confused - rejected - helpless - disappointed - insecure - anxious - other
4) joyful: - excited - thankful - happy - cheerful - relieved - hopeful - other
5) empowered: - aware - enabled - respected - appreciated - important - faithful - other
6) peaceful: - nurturing - trusting - loving - intimate - thoughtful - content - other
7) other: - none - mixed

##### Where to use
If your payload to this API is an utterance you can expect back a detail
categorical breakdown of emotions contained within it with respect to the Premera
business view.


