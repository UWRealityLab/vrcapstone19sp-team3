package arcap.speechserver;

import com.google.api.gax.core.FixedCredentialsProvider;
import com.google.auth.oauth2.GoogleCredentials;
import com.google.cloud.translate.Translate;
import com.google.cloud.translate.TranslateOptions;
import com.google.cloud.translate.Translation;

import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;

public class Translator {

    public static String translate(String text, String langCode) throws IOException {
        FileInputStream credentialsStream = new FileInputStream("./arcap.json");
        GoogleCredentials credentials = GoogleCredentials.fromStream(credentialsStream);
        FixedCredentialsProvider credentialsProvider = FixedCredentialsProvider.create(credentials);


        Translate translate = TranslateOptions.getDefaultInstance().getService();

        // Translates some text into Russian
        Translation translation =
                translate.translate(
                        text,
                        Translate.TranslateOption.sourceLanguage("en"),
                        Translate.TranslateOption.targetLanguage(langCode));


        System.out.printf("Text: %s%n", text);
        System.out.printf("Translation: %s%n", translation.getTranslatedText());
        return translation.getTranslatedText();
    }

    public static void main(String[] args) throws IOException {
        System.out.println("res: " + Translator.translate("test", "ja"));
    }

}
