import React  from 'react';
import { useDispatch } from 'react-redux'
import {Link} from 'react-router-dom'
import {FaShoppingCart} from 'react-icons/fa'
import { useTranslation } from 'react-i18next';
import {HomePurchase} from '../components/Home/HomePurchase'
import { reset} from '../features/purchases/purchaseSlice'

function Home() {
  const { t } = useTranslation(["Home"]);

  const dispatch = useDispatch();
  
  return <> 
    <Link onClick={() => dispatch(reset())} to='/new-purchase'className='btn btn-reverse btn-block'>
        <FaShoppingCart /> {t("createNewPurchase")}
    </Link>
    <Link onClick={() => dispatch(reset())} to='/new-product' className='btn btn-reverse btn-block'>
        <FaShoppingCart /> {t("createNewProduct")}
    </Link>
    <HomePurchase />
  </>;
}

export default Home;
