# Language Fingerprint from Scratch in WPF C#

##

<p align="center">
    <img src="https://github.com/grensen/language_fingerprint/blob/main/figures/lf_jm.png" >
</p>

Language Fingerprint: A very interesting idea is to visualize the relationships between characters in a language. The demo is built from scratch in WPF C# without XAML. 
You can ask your LLM or follow this guide [how to install](https://github.com/grensen/custom_connect?tab=readme-ov-file#installation). 
The demo reads characters from a website, then counts which character follows the next one. The image on the left shows the confusion matrix, while on the right is the language fingerprint, which connects characters with lines based on their frequency. 
Use the mouse wheel to switch between websites, a mouse click even animates the process.

##

<p align="center">
    <img src="https://github.com/grensen/language_fingerprint/blob/main/figures/language_fingerprint_tweet.png" >
</p>

The idea of the visualization based on [this](https://x.com/matthen2/status/1775531115874246837) tweet from Matt Henderson, where you can find more examples.

##

<p align="center">
    <img src="https://github.com/grensen/language_fingerprint/blob/main/figures/n_gram_,model.png" >
</p>

We use a so-called bigram model here, but Matt also uses a 4-gram model, which is already something like a baby LLM. Crazy stuff and good fun.

