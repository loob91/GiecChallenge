import React from 'react';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';

export const LineProductItem = ({ nameSelected, languageOptions, onChange }) => {
    const [productName, setProductName] = useState('')
    const [language, setLanguage] = useState('')
    const { t } = useTranslation(["NewProduct"]);

    useEffect(() => {  
      onChange({
        nameSelected,
        productName,
        language
      });
    }, [onChange, productName, language]);
    
    return (
      <>
        <div className="form-group">
            <label htmlFor="product">{t("product")}</label>
            <input type="text" name="productName" id="productName" className='form-control width-100' value={productName} placeholder={t("product")} onChange={(e) => setProductName(e.target.value)} />
            <label htmlFor="language">{t("language")}</label>
            <select name="language" id="language" value={language} className="form-control width-100" onChange={(e) => setLanguage(e.target.value)}>
                {languageOptions}
            </select>
        </div>
      </>
    );
  };