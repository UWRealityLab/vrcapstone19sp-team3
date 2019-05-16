package arcap.speechserver;

import com.amazonaws.auth.*;
import com.amazonaws.services.translate.AmazonTranslate;
import com.amazonaws.services.translate.AmazonTranslateClient;
import com.amazonaws.services.translate.model.TranslateTextRequest;
import com.amazonaws.services.translate.model.TranslateTextResult;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.util.Scanner;

public class Translator {

    private static boolean loadedCredentials = false;
    private static String id = null;
    private static String secret = null;

    private static AWSCredentialsProvider getCredentials() {
        if (!loadedCredentials) {
            try {
                Scanner scan = new Scanner(new File("cred.txt"));
                id = scan.nextLine();
                secret = scan.nextLine();
                loadedCredentials = true;
            } catch (FileNotFoundException e) {
                e.printStackTrace();
            }
        }
        String access_key_id = id;
        String access_key_secret = secret;
        AWSCredentials creds = new BasicAWSCredentials(access_key_id, access_key_secret);
        return new AWSStaticCredentialsProvider(creds);
    }

    public static String translate(String text, String sourceLangCode, String targetLangCode) throws IOException {
        AWSCredentialsProvider awsCreds = DefaultAWSCredentialsProviderChain.getInstance();

        AmazonTranslate translate = AmazonTranslateClient.builder()
                .withCredentials(getCredentials())
                .withRegion("us-west-2")
                .build();

        TranslateTextRequest request = new TranslateTextRequest()
                .withText(text)
                .withSourceLanguageCode(sourceLangCode)
                .withTargetLanguageCode(targetLangCode);
        TranslateTextResult result = translate.translateText(request);
        return result.getTranslatedText();
    }

    public static void main(String[] args) throws IOException {
        System.out.println("res: " + Translator.translate("test", "en", "ja"));
        System.out.println("res: " + Translator.translate("how are you doing?", "en", "ja"));
        System.out.println("res: " + Translator.translate("aujourd'hui je vais manger des frites", "fr", "en"));
    }

}
