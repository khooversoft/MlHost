The NLP Model to understand user's sentiment.


#### General Sentiment Models
Examine an utterance and return a binary 0 or 1, 0 = negative, 1 = positive


#### Fine Tuned Model for Premera
It is an atypical challenge for Premera to build a sentiment models for the
reason that language associated these customer interactions
are normally indicative negativity.  For example, "a broken knee" has
negativity in the world at large, however, within the health insurance model
a broken knee is normal.

These models also support for neutral sentiment.  Results are adjust to
* -1 negative
* 0 = neutral
* 1 = positive


#### Where to use
These models are very useful for...
1) Longitudinal studies - understanding customer feelings over time
2) Surveys - understanding people are reacting to specific events / elements.
3) Segementation - if certain demographics of our customer base are disenfrancized, this model can identifer.